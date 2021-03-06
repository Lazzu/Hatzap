#version 330

in vec2 tcoord;
in vec3 norm;
in vec4 fcolor;

uniform samplerCube textureSampler;
uniform float time;
uniform vec4 color = vec4(1,1,1,1);
uniform float refractionRatio = 1.333;
uniform float ReflectionRefraction = 0.0;

layout(location = 0) out vec4 RGBA;

const float gamma = 1.0 / 2.2;

void main( void )
{
    vec3 n = normalize(norm);
	vec3 l = normalize(vec3(0.25,10,20));
	vec3 e = normalize(vec3(0,20,200));
	vec3 h = normalize(e + l);
	
	float nDotL = dot(n, l);
	float nDotH = dot(n, h);
	float light = nDotL;
	float specular = pow( max(0.0, nDotH), 64 );
	
	vec4 fragcolor = color * fcolor;

	vec3 refltexel = texture2D(textureSampler, reflect(e, n)).rgb;
	vec3 refrtexel = texture2D(textureSampler, refract(e, n, refractionRatio)).rgb;
	
	vec3 texel = lerp(refltexel, refrtexel, min(1.0, max(0.0, ReflectionRefraction)));
	
	vec3 finalColor = pow(texel, vec3(gamma)) * light * fragcolor.rgb + specular;

	finalColor = pow(finalColor, vec3(2.2));
	finalColor = clamp(finalColor, vec3(0.0), vec3(1.0));
	
  RGBA = vec4(finalColor, fragcolor.a);
	//RGBA = vec4(texel, 1);
	//RGBA = vec4(tcoord, 0, 1);
	//RGBA = vec4(value, value, value, 1);
}

