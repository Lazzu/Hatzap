#version 330

precision highp float;

in vec2 fTexcoord;
in vec4 fColor;
in vec3 fNormal;
in vec3 fPos;

uniform float gamma = 2.2;
uniform vec3 EyeDirection;
uniform sampler2DArray textureSampler;
uniform vec4 Color = vec4(1.0, 0.0, 0.0, 1);

layout(location = 0) out vec4 RGBA;
layout(location = 1) out vec3 Normals;
layout(location = 2) out vec4 Material;
layout(location = 3) out vec4 Effects; // X = motion blur, Y = motion blur, Z = Self-illuminated pixels, W = ???


vec2 BlinnPhongDirectional(vec3 eyeDir, vec3 lightDir, vec3 normal, vec4 MaterialValues)
{
	float nDotL = dot(normal, lightDir);
	float nDotH = dot(normal, normalize(eyeDir + lightDir));
	float diffuse = max(nDotL, 0.0);
	float specular = pow(clamp(nDotH, 0.0, 1.0), MaterialValues.x);
	return vec2(diffuse, specular);
}

vec3 rim(vec3 color, float start, float end, float coef)
{
  vec3 normal = normalize(fNormal);
  vec3 eye = normalize(-EyeDirection);
  float rim = smoothstep(start, end, 1.0 - dot(normal, eye));
  return clamp(rim, 0.0, 1.0) * coef * color;
}

void main( void )
{
	float g = 1 / gamma;
		
	vec4 material;
	material.x = 40.0;
	vec3 ed = -EyeDirection;
	ed = normalize(ed);
	
	vec3 texel = texture(textureSampler, vec3(fTexcoord, 0)).xyz;
	
	vec3 ld = normalize(vec3(1,1,1));
	
	vec2 lights = BlinnPhongDirectional(ed, ld, normalize(fNormal), material);
	
    vec3 diffColor = Color.rgb;
	vec3 riml = rim(diffColor, 0.0, 1.0, 0.5) ;
  
	vec3 outColor = texel * diffColor * lights.x + lights.y + riml;
	//outColor = pow(outColor, vec3(g));
	RGBA = vec4(outColor, Color.a);
	//RGBA = outColor;
}

