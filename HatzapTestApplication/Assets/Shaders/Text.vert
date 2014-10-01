#version 330

layout(location = 0) in vec2 vertex;
layout(location = 1) in vec2 uv;
layout(location = 2) in vec4 vColor;
layout(location = 3) in vec4 vBorderColor;
layout(location = 4) in vec3 vSettings;

uniform mat4 MVP;
uniform vec2 textureSize;
uniform float Size;
uniform vec4 Settings;

out vec2 texcoord;
out vec4 color;
out vec4 borderColor;
out vec3 settings;

void main()
{
	texcoord = uv / textureSize;
	color = vColor;
	borderColor = vBorderColor;
	settings = Settings.yzw;

	gl_Position = MVP * vec4(vec3(vertex * vec2(Settings.x, Settings.x), 0), 1);
}

