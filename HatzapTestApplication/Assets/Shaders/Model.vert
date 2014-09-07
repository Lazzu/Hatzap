#version 330

layout(location = 0) in vec3 vertex;
layout(location = 1) in vec3 normal;
layout(location = 2) in vec3 tangent;
layout(location = 3) in vec3 binormal;
layout(location = 4) in vec2 uv;
layout(location = 5) in vec4 color;

uniform mat4 MVP;

out vec3 outNormal;
out vec2 outUV;

void main( void )
{
	vec4 rotated = MVP * vec4(normal, 0);
	//vec4 rotated = vec4(normal, 0);

	outNormal = (rotated.xyz + vec3(1.0)) * vec3(0.5);
	//outNormal = rotated.xyz;
	//outNormal = tangent.xyz;
	//outNormal = binormal.xyz;
	//outNormal = color.xyz;
	//outNormal = vec3(uv, 0.0);
	//outNormal = vec3(1.0, 0.0, 0.0).xyz;

	gl_Position = MVP * vec4(vertex, 1);
}

