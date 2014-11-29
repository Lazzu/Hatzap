#version 330

layout(triangles) in;
layout(triangle_strip, max_vertices = 3) out;

in vec2 gTexcoord[];
in vec4 gColor[];
in vec3 gNormal[];

out vec2 fTexcoord;
out vec4 fColor;
out vec3 fNormal;

bool TestCollision(vec3 aCenter, vec3 aSize, vec3 bCenter, vec3 bSize)
{
	vec3 center = abs(aCenter - bCenter);
	vec3 size = aSize + bSize;
	return !(center.x > size.x || center.y > size.y || center.z > size.z);
}

void main() {

	// Generate AABB
	vec3 minValue = gl_in[0].gl_Position.xyz / gl_in[0].gl_Position.w;
	vec3 maxValue = gl_in[0].gl_Position.xyz / gl_in[0].gl_Position.w;
	minValue = min(minValue, gl_in[1].gl_Position.xyz / gl_in[1].gl_Position.w);
	maxValue = max(maxValue, gl_in[1].gl_Position.xyz / gl_in[1].gl_Position.w);
	minValue = min(minValue, gl_in[2].gl_Position.xyz / gl_in[2].gl_Position.w);
	maxValue = max(maxValue, gl_in[2].gl_Position.xyz / gl_in[2].gl_Position.w);
	vec3 aSize = maxValue - minValue;
	vec3 aCenter = minValue + aSize * 0.5;
	
	// Emit if inside screen
	if ( TestCollision(aCenter, aSize, vec3(0,0,0), vec3(1,1,1) ) )
	{
		for(int i = 0; i < 3; i++)
		{
			fTexcoord = gTexcoord[i];
			fColor = gColor[i];
			fNormal = gNormal[i];
			gl_Position = gl_in[i].gl_Position;
			EmitVertex();
		}
		EndPrimitive();
	}
}
