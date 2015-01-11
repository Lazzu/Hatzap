#version 330

in vec2 tcoord;
in vec3 norm;
in vec4 fcolor;

uniform sampler2D textureSampler;
uniform float time;
uniform vec4 color = vec4(1,1,1,1);
uniform vec3 EyeDirection;

layout(location = 0) out vec4 RGBA;

const float gamma = 1.0 / 2.2;

vec2 calcLight(vec3 n, vec3 l, vec3 e)
{
  float nDotL = max(dot(n, normalize(l)), 0.0);
  vec3 r = normalize(-reflect(l,n));
	float light = nDotL;
	float specular = pow( max(dot(r,e), 0.0), 64 );
	return vec2(light, specular);
}

void main( void )
{
  // Light positions
	vec3 l1 = vec3(25,10,20);
	vec3 l2 = vec3(-25,10,-20);
	
	// Lighting calculations for each light
	vec2 light1 = calcLight(norm, l1, EyeDirection);
	vec2 light2 = calcLight(norm, l2, EyeDirection);
	
	// Add lights up
	vec2 light = light1 + light2;
	
	// Calculate final fragment color, assume white light
	vec4 fragcolor = color * fcolor;
	vec3 finalColor = light.x * fragcolor.rgb + light.y;

  // Gamma-correction and clamping
	finalColor = pow(finalColor, vec3(2.2));
	finalColor = clamp(finalColor, vec3(0.0), vec3(1.0));
	
	// Output the color
  RGBA = vec4(finalColor, fragcolor.a);
}

