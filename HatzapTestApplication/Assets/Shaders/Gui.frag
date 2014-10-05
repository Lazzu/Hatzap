#version 330

in vec3 tcoord;
in vec4 color;

uniform sampler2DArray textureSampler;

layout(location = 0) out vec4 RGBA;

void main( void )
{
    RGBA = texture(textureSampler, tcoord) * color;
	//RGBA = vec4(1,1,1,1);
}

