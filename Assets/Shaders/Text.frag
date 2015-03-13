#version 330

in vec2 texcoord;
in vec4 color;
in vec4 borderColor;
in vec3 settings;

uniform sampler2D textureSampler;
uniform float gamma = 2.2;

layout(location = 0) out vec4 RGBA;



void main( void )
{
	float weight = settings.x;
	float border = settings.y;
	float smoothness = settings.z;

    float sdf = texture2D(textureSampler, texcoord.xy).r;
	
	sdf = clamp(weight * sdf, 0.0, 1.0);

	// use current drawing color
	vec4 basecolor = color;
 
	// do some anti-aliasing
	basecolor.a *= smoothstep(0.5 - smoothness, 0.5 + smoothness, sdf);
	basecolor.a = pow(basecolor.a, 1.0/gamma);



	// final color
	RGBA = basecolor; 
	//RGBA = texel;
	//RGBA = vec4(texcoord, 0, 1);
	//RGBA = color;
	//RGBA = vec4(1);
	//RGBA = vec4(sdf, sdf, sdf,1);











	/*
	float distAlphaMask = baseColor.a; 

	if ( OUTLINE &&	( distAlphaMask >= OUTLINE_MIN_VALUE0 ) && ( distAlphaMask <= OUTLINE_MAX_VALUE1 ) )
	{
		float oFactor = 1.0;

		if ( distAlphaMask <= OUTLINE_MIN_VALUE1 )
		{
			oFactor = smoothstep(OUTLINE_MIN_VALUE0, OUTLINE_MIN_VALUE1, distAlphaMask);
		}
		else
		{
			oFactor = smoothstep(OUTLINE_MAX_VALUE1, OUTLINE_MAX_VALUE0, distAlphaMask);
		}

		baseColor = lerp(baseColor, OUTLINE_COLOR, oFactor) ;
	}

	if (SOFT_EDGES) 
	{
		baseColor.a ∗= smoothstep(SOFT_EDGE_MIN, SOFT_EDGE_MAX, distAlphaMask) ;
	} 
	else 
	{
		baseColor.a = distAlphaMask >= 0.5;
	} 

	if (OUTER_GLOW) 
	{
		float4 glowTexel = tex2D ( BaseTextureSampler , i.baseTexCoord.xy + GLOW_UV_OFFSET);
		float4 glowc = OUTER_GLOW_COLOR ∗ smoothstep(OUTER_GLOW_MIN_DVALUE, OUTER_GLOW_MAX_DVALUE, glowTexel.a);
		baseColor = lerp(glowc, baseColor, mskUsed);
	}
	*/
}

