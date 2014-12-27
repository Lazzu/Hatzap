#version 330

in vec2 tcoord;

uniform sampler2D textureSampler;

layout(location = 0) out vec4 RGBA;

void main( void )
{
    RGBA = vec4(texture(textureSampler, tcoord).rgb, 1);
	//RGBA = vec4(tcoord, 0, 1);
}

