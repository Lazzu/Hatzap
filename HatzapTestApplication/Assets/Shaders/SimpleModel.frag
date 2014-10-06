#version 330

in vec2 texcoord;
in vec4 vColor;
in vec3 vNormal;
in vec3 pos;

uniform float gamma = 2.0;
uniform vec3 EyeDirection;
uniform sampler2DArray textureSampler;
uniform vec4 Color = vec4(0.2, 0.8, 0.8, 1);

layout(location = 0) out vec4 RGBA;
layout(location = 1) out vec3 Normals;
layout(location = 2) out vec4 Material;
layout(location = 3) out vec4 Effects; // X = motion blur, Y = motion blur, Z = Self-illuminated pixels, W = ???


void main( void )
{
	//float g = 1;
	float g = 1 / gamma;
	//float g = gamma;
	
	vec3 lightDir = normalize(vec3(1,1,1));
	
	vec3 half = normalize(EyeDirection + lightDir);
	
	float nDotL = dot(normalize(vNormal), normalize(lightDir));
	float nDotH = dot(normalize(vNormal), normalize(half));
	
	float specular = 0;
	
	if(nDotL > 0)
		specular = pow(clamp(0.0, 1.0, nDotH), 1);
	
	float diffuse = clamp(0.0, 1.0, nDotL);
		
	vec4 outColor = Color * diffuse + specular;

	RGBA = outColor;
	//RGBA = outColor;
}

