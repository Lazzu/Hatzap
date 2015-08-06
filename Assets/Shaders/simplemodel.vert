#version 330

layout(location = 0) in vec4 color;
layout(location = 1) in vec3 vertex;
layout(location = 2) in vec3 normal;
layout(location = 3) in vec3 tangent;
layout(location = 4) in vec3 uv;
layout(location = 5) in mat4 mInstancedModelMatrix;

out vec2 tcoord;
out vec3 norm;
out vec4 fcolor;

uniform mat4 mViewProjection;
uniform mat4 mModel;
uniform mat3 mNormal;
uniform float time;

void main()
{
	tcoord = uv.xy;
	fcolor = color;
	norm = mNormal * mat3(mModel) * normal;
	gl_Position = mViewProjection * mModel * vec4(vertex , 1);
}

