#version 330

layout(location = 0) in vec4 color;
layout(location = 1) in vec3 vertex;
layout(location = 2) in vec3 normal;
layout(location = 3) in vec3 tangent;
layout(location = 4) in vec3 uv;
layout(location = 5) in mat4 mInstancedModelMatrix;

out vec2 tcoord;
out vec3 norm;
out float theTime;

uniform mat4 mViewProjection;
uniform mat4 mModel;
uniform mat3 mNormal;
uniform float time = 0;
uniform vec2 toffset = vec2(0,0);

void main()
{
	tcoord = (vertex.xy + (toffset * 0.1) - vec2(time * 0.01, 0)) ;
	norm = mat3(mModel) * normal;
	vec3 offset = vec3( 0.0, sin((time + vertex.x * 20 + vertex.z * 20)) * 0.05, 0.0 );
	vec3 tmp = vertex + offset;
	gl_Position = mViewProjection * mModel * vec4(tmp, 1);
}

