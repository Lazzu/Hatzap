#version 330
#extension GL_ARB_draw_instanced : enable

precision highp float;

layout(location = 0) in mat4 mInstancedModelMatrix;
layout(location = 4) in vec4 color;
layout(location = 5) in vec3 vertex;
layout(location = 6) in vec3 normal;
layout(location = 7) in vec3 tangent;
layout(location = 8) in vec2 uv;



uniform mat4 mViewProjection;
uniform mat4 mModel;
uniform mat3 mNormal;

out vec2 gTexcoord;
out vec4 gColor;
out vec3 gNormal;

void main( void )
{	
	gTexcoord = uv;

	gColor = color;
	vec3 binormal = cross(normal, tangent);
	//vNormal = mNormal * normal;
	gNormal = normalize(normal);
	
	gl_Position = mViewProjection * mInstancedModelMatrix * vec4(vertex, 1);
	//gl_Position = mViewProjection * mModel * vec4(vertex, 1);
}

