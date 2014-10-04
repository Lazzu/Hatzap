#version 330

layout(location = 0) in vec2 vertex;
layout(location = 1) in vec2 uv;
layout(location = 2) in uint page;

uniform vec2 TextureSize;
uniform mat4 Projection;

out vec3 tcoord;

void main()
{
	tcoord = vec3(uv / TextureSize, page);
	gl_Position = Projection * vec4(vec3(vertex, 0), 1);
}

