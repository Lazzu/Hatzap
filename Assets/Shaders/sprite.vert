#version 330

layout(location = 0) in vec3 vertex;
layout(location = 1) in vec2 uv;
layout(location = 2) in vec3 position;
layout(location = 3) in vec2 size;
layout(location = 4) in float rotation;

out vec2 tcoord;

uniform mat4 projection;

mat4 ConstructModelMatrix(vec3 position, vec2 size, float rotation)
{
  float rSin = sin(rotation);
  float rCos = cos(rotation);
  
  mat4 r = mat4(
    vec4(rCos, rSin, 0, 0),
    vec4(-rSin, rCos, 0, 0),
    vec4(0, 0, 1, 0),
    vec4(0, 0, 0, 1)
  );
  mat4 t = mat4(
    vec4(1, 0, 0, 0),
    vec4(0, 1, 0, 0),
    vec4(0, 0, 1, 0),
    vec4(position, 1)
  );
  mat4 s = mat4(
    vec4(size.x, 0, 0, 0),
    vec4(0, size.y, 0, 0),
    vec4(0, 0, 1, 0),
    vec4(0, 0, 0, 1)
  );

  return t * r * s;
}

void main()
{
  mat4 mM = ConstructModelMatrix(position, size, rotation);
  
	tcoord = uv;
	gl_Position = projection * mM * vec4(vertex, 1);
}

