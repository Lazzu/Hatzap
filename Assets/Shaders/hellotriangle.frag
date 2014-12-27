#version 330

in vec3 color;

layout(location = 0) out vec4 RGBA;

void main( void )
{
	RGBA = vec4(color, 1);
	//RGBA = vec4(1,1,1,1);
}

