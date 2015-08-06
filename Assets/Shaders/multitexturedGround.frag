#version 330

in vec2 tcoord;
in vec3 norm;

uniform sampler2D stoneTexture;
uniform sampler2D grassTexture;

uniform float time;
uniform vec4 color = vec4(1,1,1,1);
uniform mat3 mNormal;
uniform vec3 EyeDirection;
uniform float specularIntensity;

layout(location = 0) out vec4 RGBA;

const float gamma = 1.0 / 2.2;

vec2 calculateLighting(vec3 normal, vec3 lightdir, vec3 eyedir)
{
	vec3 halve = normalize(lightdir + eyedir);
	float nDotL = dot(normal, lightdir);
	float nDotH = dot(normal, halve);
	float light = nDotL;
	float specular = pow( max(0.0, nDotH), 64 );
	return vec2(light, specular);
}

void main( void )
{
    vec3 n = normalize(norm);
	vec3 l1 = normalize(mNormal * vec3(0.25,15,20));
	vec3 l2 = normalize(mNormal * vec3(-0.25,15,-20));
	vec3 e = -EyeDirection;
	
	vec2 light1 = calculateLighting(n, l1, e);
	vec2 light2 = calculateLighting(n, l2, e);
	vec2 finalLight = light1 + light2;
	
	vec3 stone = texture2D(stoneTexture, tcoord).rgb;
	vec3 grass = texture2D(grassTexture, tcoord).rgb;
	
	float coord = clamp(tcoord.x, 0, 1);
	
	vec3 texel = stone * coord + grass * (1 - coord);
	
	vec3 finalColor = pow(texel, vec3(gamma)) * finalLight.x * color.rgb + finalLight.y * specularIntensity;

	finalColor = pow(finalColor, vec3(2.2));
	finalColor = clamp(finalColor, vec3(0.0), vec3(1.0));
	
    RGBA = vec4(finalColor * color.a, color.a * 0.5);
	//RGBA = vec4(texel, 1);
	//RGBA = vec4(value, value, value, 1);
}

