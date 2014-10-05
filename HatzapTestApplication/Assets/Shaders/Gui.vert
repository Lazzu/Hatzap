#version 330

layout(location = 0) in vec4 vColor;
layout(location = 1) in vec2 vertex;
layout(location = 2) in vec2 uv;
layout(location = 3) in uint page;

uniform vec2 TextureSize;
uniform mat4 Projection;

out vec3 tcoord;
out vec4 color;

void main()
{
	color = vColor;
	tcoord = vec3(uv / TextureSize, page);
	gl_Position = Projection * vec4(vec3(vertex, 0), 1);
}

