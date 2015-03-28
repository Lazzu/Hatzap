#version 330

in vec2 tcoord;

uniform sampler2D textureSampler;

layout(location = 0) out vec4 RGBA;

void main( void )
{
	RGBA = texture2D(textureSampler, tcoord);
}

