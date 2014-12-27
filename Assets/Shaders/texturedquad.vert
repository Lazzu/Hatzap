#version 330

layout(location = 0) in vec3 vertex;

out vec2 tcoord;

void main()
{
	tcoord = (vertex.xy + vec2(1.0,1.0)) / 2.0;
	
	gl_Position = vec4(vertex, 1);
}

