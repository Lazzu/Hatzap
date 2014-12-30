#version 330

layout(location = 0) in vec3 vertex;
layout(location = 1) in vec3 uv;

uniform mat4 mViewProjection;

out vec2 tcoord;

void main()
{
	tcoord = uv.xy;
	
	//gl_Position = mViewProjection * vec4(vertex, 1);
	gl_Position = vec4(vertex, 1);
}

