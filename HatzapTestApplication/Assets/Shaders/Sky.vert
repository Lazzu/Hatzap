#version 330

layout(location = 0) in vec3 vertexPosition_modelspace;

uniform mat4 InvProjection;
uniform mat4 InvViewRotation;

uniform vec4 paska;

out vec3 ViewDirection;

void main( void )
{
	vec4 device_normal = vec4(vertexPosition_modelspace.xy, 0.0, 1.0);
    vec4 eye_normal = normalize(InvProjection * device_normal);
    vec4 world_normal = normalize(InvViewRotation * eye_normal);

	ViewDirection = device_normal.xyz;

	gl_Position = vec4(vertexPosition_modelspace, 1);
}

