#version 330

precision highp float;

layout(location = 0) in vec3 vertex;
layout(location = 1) in vec3 normal;
layout(location = 2) in vec3 tangent;
layout(location = 3) in vec3 binormal;
layout(location = 4) in vec2 uv;
layout(location = 5) in vec4 color;

uniform mat4 MVP;
uniform mat3 mN;

out vec2 texcoord;
out mat3 TBN;
out vec4 vColor;
out vec3 vNormal;

void main( void )
{
	//mat3 mN = mat3(MVP);
	
	texcoord = uv;
	
	vec3 n = normalize(mN * normal);
	vec3 t = normalize(mN * tangent);
	vec3 b = normalize(mN * binormal);
	
	TBN = transpose(mat3(t, b, n));

	vColor = color;
	//vColor = vec4(((mN * normal) + vec3(1)) * vec3(0.5), 1) * color;
	vNormal = mN * normal;

	gl_Position = MVP * vec4(vertex, 1);
}

