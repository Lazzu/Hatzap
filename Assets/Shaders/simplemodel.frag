#version 330

in vec2 tcoord;
in vec3 norm;

uniform sampler2D textureSampler;

layout(location = 0) out vec4 RGBA;

const float gamma = 1.0 / 2.2;

void main( void )
{
	float light = dot(norm, normalize(vec3(0.25,1,2)));

	vec3 finalColor = pow(texture2D(textureSampler, tcoord).rgb, vec3(gamma)) * light;

	finalColor = pow(finalColor, vec3(2.2));
	finalColor = clamp(finalColor, vec3(0.0), vec3(1.0));
	
    RGBA = vec4(finalColor, 1);
	//RGBA = vec4(norm, 1);
	//RGBA = vec4(light, light, light, 1);
}

