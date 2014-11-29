#version 330

in vec2 fTexcoord;
in vec4 fColor;
in vec3 fNormal;

uniform float gamma = 2.0;
uniform vec3 EyeDirection;
uniform sampler2DArray textureSampler;
uniform vec4 Color = vec4(1.0, 0.0, 0.0, 1);

layout(location = 0) out vec4 RGBA;
layout(location = 1) out vec3 Normals;
layout(location = 2) out vec4 Material;
layout(location = 3) out vec4 Effects; // X = motion blur, Y = motion blur, Z = Self-illuminated pixels, W = ???


void main( void )
{
	//float g = 1;
	float g = 1 / gamma;
	//float g = gamma;
	
	vec3 texel = texture(textureSampler, vec3(fTexcoord, 0)).xyz;
	
	vec3 lightDir = normalize(vec3(1,1,1));
	
	//vec3 half = normalize(EyeDirection + lightDir);
	
	float nDotL = dot(normalize(fNormal), normalize(lightDir));
		
	float diffuse = clamp(0.0, 1.0, nDotL);
		
	vec3 outColor = Color.rgb * texel * diffuse;

	RGBA = vec4(outColor, Color.a);
	//RGBA = outColor;
}

