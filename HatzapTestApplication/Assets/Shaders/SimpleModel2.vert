#version 330
#extension GL_ARB_draw_instanced : enable

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

out vec2 fTexcoord;
out vec4 fColor;
out vec3 fNormal;
out vec3 fPos;

void main( void )
{
	fTexcoord = uv;

	mat3 mM = mat3(mInstancedModelMatrix);
	
	fColor = color;
	vec3 binormal = cross(normal, tangent);
	fNormal = mM * normal;
	//fNormal = normalize(normal);
	
	vec4 pos = mInstancedModelMatrix * vec4(vertex, 1);
	fPos = pos.xyz;
	
	gl_Position = mViewProjection * pos;
	//gl_Position = mViewProjection * mModel * vec4(vertex, 1);
}

