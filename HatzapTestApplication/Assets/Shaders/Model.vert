#version 330

precision highp float;

layout(location = 0) in vec4 color;
layout(location = 1) in vec3 vertex;
layout(location = 2) in vec3 normal;
layout(location = 3) in vec3 tangent;
layout(location = 4) in vec2 uv;
layout(location = 5) in mat4 mInstancedModelMatrix;

uniform mat4 mViewProjection;
uniform mat4 mModel;
uniform mat3 mNormal;

out vec2 texcoord;
out mat3 TBN;
out vec4 vColor;
out vec3 vNormal;

void main( void )
{
	texcoord = uv;
	
	vec3 binormal = cross(normal, tangent);
	
	vec3 n = normalize(mNormal * normal);
	vec3 t = normalize(mNormal * tangent);
	vec3 b = normalize(mNormal * binormal);
	
	TBN = transpose(mat3(t, b, n));

	vColor = color;
	vNormal = normal;

	vec4 pos = mInstancedModelMatrix * vec4(vertex, 1);
	
	gl_Position = mViewProjection * pos;
}

