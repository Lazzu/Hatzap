#version 330

layout(location = 0) in vec2 vertex;

uniform mat4 Projection;
uniform vec2 Position;
uniform vec2 Size;

out vec2 tcoord;

void main()
{
	
	
	vec2 vert = vertex * Size + Position;
	
	tcoord = vertex;
	
	gl_Position = Projection * vec4(vec3(vert, 0), 1);
}

