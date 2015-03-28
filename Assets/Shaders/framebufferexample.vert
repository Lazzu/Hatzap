#version 330

layout(location = 0) in vec3 vertex;

out vec2 tcoord;

void main()
{
	tcoord = ( vertex.xy + vec2(1,1) ) * vec2(0.5, 0.5);
	gl_Position = vec4(vertex, 1);
}

