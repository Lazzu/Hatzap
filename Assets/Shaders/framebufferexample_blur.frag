#version 330

in vec2 tcoord;

uniform vec2 ScreenSize;
uniform sampler2D textureSampler;

layout(location = 0) out vec4 RGBA;

float invCount = 1.0 / 49.0;

void main( void )
{
	vec4 outcolor = vec4(0,0,0,0);
	vec2 screenMultiplier = 1.0 / ScreenSize;
	
	// Average blur
	for(int y = -3; y < 4; y++)
	for(int x = -3; x < 4; x++)
	{
		outcolor += texture2D(textureSampler, tcoord + (vec2(x,y) * screenMultiplier));
	}
	RGBA = outcolor * invCount;
}

