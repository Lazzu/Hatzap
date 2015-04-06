#version 330

in vec2 tcoord;

uniform sampler2DMS textureSampler;
uniform int MSAA_Samples;

layout(location = 0) out vec4 RGBA;

void main( void )
{
	vec2 tmp = floor(textureSize( textureSampler ) * tcoord);
	ivec2 coord = ivec2(tmp.x, tmp.y);

	float invMsaa = 1.0 / MSAA_Samples;
	
	vec4 outColor = vec4(0,0,0,0);
	
	for(int i = 0; i < MSAA_Samples; i++)
	{
		outColor += texelFetch(textureSampler, coord, i);
	}
	
	RGBA = outColor * vec4(invMsaa, invMsaa, invMsaa, invMsaa);
	
	
}

