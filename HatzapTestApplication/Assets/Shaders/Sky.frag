#version 330

in vec3 ViewDirection;

uniform samplerCube SkyTexture;

out vec4 Color;

void main( void )
{
	Color = vec4(ViewDirection, 1);
}

