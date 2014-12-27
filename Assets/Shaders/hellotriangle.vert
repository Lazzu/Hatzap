#version 330

layout(location = 0) in vec3 vertex;

out vec3 color;

void main()
{
	// Very hacky color selection based on vertex position
	color = vec3(0,1,0);
	if(vertex.y < 0)
	{
		color.g = 0;
		if(vertex.x > 0)
		{
			color.r = 1;
		}
		else
		{
			color.b = 1;
		}
	}
	
	gl_Position = vec4(vertex, 1);
}

