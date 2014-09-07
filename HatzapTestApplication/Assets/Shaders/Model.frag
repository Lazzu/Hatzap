#version 330

in vec3 outNormal;
in vec2 outUV;

out vec4 Color;

void main( void )
{
	//Color = vec4((outNormal + vec3(1.0)) * vec3(0.5), 1) ;
	Color = vec4(outNormal, 1);
	//Color = vec4(outUV.x, outUV.y, 0, 1);
}

