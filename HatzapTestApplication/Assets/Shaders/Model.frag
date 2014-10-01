#version 330

in vec2 texcoord;
in mat3 TBN;
in vec4 vColor;
in vec3 vNormal;

uniform float gamma = 2.2;
uniform vec3 EyeDirection;
uniform sampler2D textureSampler;

layout(location = 0) out vec4 RGBA;
layout(location = 1) out vec3 Normals;
layout(location = 2) out vec4 Material;
layout(location = 3) out vec4 Effects; // X = motion blur, Y = motion blur, Z = Self-illuminated pixels, W = ???

//Material.x=Roughness
vec2 oren_nayar(vec3 LightNormal,vec3 SurfaceNormal,vec3 EyeNormal,vec4 Material) {
	float gamma=dot(EyeNormal -	SurfaceNormal * dot(EyeNormal, SurfaceNormal),
		-LightNormal -	SurfaceNormal * dot(-LightNormal, SurfaceNormal));
	float rough_sq = Material.x * Material.x;
	float A = 1.0 - 0.5 * (rough_sq / (rough_sq + 0.57));
	float B = 0.45 * (rough_sq / (rough_sq + 0.09));
	float alpha = max(acos(dot(EyeNormal, SurfaceNormal)), acos(dot(-LightNormal, SurfaceNormal)));
	float beta  = min(acos(dot(EyeNormal, SurfaceNormal)), acos(dot(-LightNormal, SurfaceNormal)));
	float C = sin(alpha) * tan(beta);
	float final = A + B * max(0.0, gamma) * C;
	final *= max(0.0, dot(SurfaceNormal, -LightNormal));
	return vec2(final, 0.0);
}

const float kf=1.12;
const float ks=1.01;

float fresnel(float x) {
	float _kf2=1.0/(kf*kf);
	float num=x-kf;
	num=1.0/(num*num)-_kf2;
	float den=1.0-kf;
	den=1.0/(den*den)-_kf2;
	return num/den;
}

float shadow(float x) {
	float num=1.0-ks;
	num=1.0/(num*num);
	float x_ks=x-ks;
	num-=1.0/(x_ks*x_ks);
	float den=1.0-ks;
	den=1.0/(den*den);
	den-=1.0/(ks*ks);
	return num/den;
}


//Material.x=Smoothness
//Material.y=Metalness
//Material.z=Index Of Refraction
//Material.w=Alpha
vec2 strauss(vec3 LightNormal,vec3 SurfaceNormal,vec3 EyeNormal,vec4 Material) {
	vec3 h = reflect(-LightNormal, SurfaceNormal);

	// Declare any aliases:
	float NdotL   = -dot(SurfaceNormal, LightNormal);
	float NdotV   = dot(SurfaceNormal, EyeNormal);
	float HdotV   = dot(h, EyeNormal);
	float fNdotL  = fresnel(NdotL);
	float s_cubed = Material.x * Material.x * Material.x;

	// Evaluate the diffuse term
	float d  = (1.0 - Material.y * Material.x);
	float Rd = (1.0 - s_cubed) * (1.0 - Material.w);
	float diffuse = NdotL * d * Rd;

	// Compute the inputs into the specular term
	float r = (1.0 - Material.w) - Rd;
	float j = fNdotL * shadow(NdotL) * shadow(NdotV);

	// 'k' is used to provide small off-specular
	// peak for very rough surfaces. Can be changed
	// to suit desired results...
	const float k = 0.1;
	float specular = min(1.0, r + j * (r + k));
	specular *= pow(-HdotV, 3.0 / (1.0 - Material.x));
	
	return max(vec2(0.0), vec2(diffuse, specular));
}

//Material.x=Specular Exponent
//Material.y=Specular Factor
vec2 phong_blinn(vec3 LightNormal, vec3 SurfaceNormal, vec3 EyeNormal, vec4 Material) {
	vec3 halfVec = normalize(EyeNormal - LightNormal);
	float diffuse = max(0.0, -dot(SurfaceNormal, LightNormal));
	float spec = pow(max(0.0, dot(halfVec, SurfaceNormal)), Material.x) * Material.y;
	return vec2(diffuse, spec);
}

//Material.x=Anisotropy (X)
//Material.y=Anisotropy (Y)
vec2 ashikhmin_shirley(vec3 LightNormal,vec3 SurfaceNormal,vec3 EyeNormal,vec4 Material) {
	vec3 h = normalize(EyeNormal - LightNormal);

	// Define the coordinate frame
	vec3 epsilon = vec3(1.0, 0.0, 0.0);
	vec3 tangent = normalize(cross(SurfaceNormal, epsilon));
	vec3 bitangent = normalize(cross(SurfaceNormal, tangent));

	// Generate any useful aliases
	float VdotN = dot(EyeNormal, SurfaceNormal);
	float LdotN = -dot(LightNormal, SurfaceNormal);
	float HdotN = dot(h, SurfaceNormal);
	float HdotL = -dot(h, LightNormal);
	float HdotT = dot(h, tangent);
	float HdotB = dot(h, bitangent);

	// Compute the diffuse term
	float diffuse = 0.7 * 28.0 / (23.0 * 3.14159);
	diffuse *= 1.0 - pow(1.0 - (LdotN * 0.5), 5.0);
	diffuse *= 1.0 - pow(1.0 - (VdotN * 0.5), 5.0);

	// Compute the specular term
	float spec = (Material.x * HdotT * HdotT + Material.y * HdotB * HdotB);
	spec = pow(HdotN, spec / (1.0 - HdotN * HdotN));
	spec *= 0.3 * sqrt((Material.x + 1.0) * (Material.y + 1.0));
	spec /= 8.0 * 3.14159 * HdotL * max(LdotN, VdotN);
	spec *= 0.3 + 0.7 * pow(1.0 - HdotL, 5.0);

	return max(vec2(0.0), vec2(diffuse, spec));
}

void main( void )
{
	float g = 1 / gamma;

	vec2 front = vec2(0);
	vec2 top = vec2(0);
	vec2 side = vec2(0);

	vec3 n = vNormal;

	//Material.x=Anisotropy (X)
	//Material.y=Anisotropy (Y)
	vec4 material = vec4(500,500,0,0);

	front = ashikhmin_shirley(normalize(vec3(0, 0, -1)), n, -EyeDirection, material);
	top = ashikhmin_shirley(normalize(vec3(0, -1, 0)), n, -EyeDirection, material);
	side = ashikhmin_shirley(normalize(vec3(1, 0, 0)), n, -EyeDirection, material);

	vec2 tmp = clamp(front + top + side, vec2(0), vec2(1));

	vec4 diffuse = vec4(tmp.x, tmp.x, tmp.x, 1);
	vec4 specular = vec4(tmp.y, tmp.y, tmp.y, 1);

	RGBA = diffuse + specular;

	//RGBA = vec4(texcoord, 0, 1);
	//RGBA = vec4((n + 1) * 0.5, 1);
	//RGBA = vec4(n , 1);

	//RGBA = texture( textureSampler, texcoord ) * diffuse + specular;
	//RGBA = texture( textureSampler, texcoord );
	//RGBA = pow(texture( textureSampler, texcoord ), vec4(g, g, g, 1));
	//RGBA = vec4(1);
	//Normals = ((TBN * texture( textureSampler, vec3(texcoord, 1.0) ).xyz) + vec3(1)) * vec3(0.5);

}

