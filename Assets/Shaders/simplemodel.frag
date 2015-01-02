#version 330

in vec2 tcoord;
in vec3 norm;

uniform sampler2D textureSampler;

layout(location = 0) out vec4 RGBA;

void main( void )
{
	float light = dot(norm, vec3(1,1,0));

	light = max(0.25, light);
	
    RGBA = vec4(light * texture2D(textureSampler, tcoord).rgb, 1);
	//RGBA = vec4(norm, 1);
	//RGBA = vec4(light, light, light, 1);
}

