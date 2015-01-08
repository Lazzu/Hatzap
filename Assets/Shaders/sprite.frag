#version 330

in vec2 tcoord;

uniform sampler2D textureSampler;

layout(location = 0) out vec4 RGBA;

void main( void )
{
  RGBA = texture(textureSampler, tcoord);
	//RGBA = vec4(1, 1, 1, 1);
}

