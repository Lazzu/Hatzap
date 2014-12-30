using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Hatzap.Utilities
{
    [ObsoleteAttribute("ScreenFiller is obsolete and will be removed at some point in future. Use VertexBatch with custom shader instead.", false)]
    public static class ScreenFiller
    {
        static Vector3[] vertices = new Vector3[] {
            new Vector3(-1, -1, 0),
            new Vector3(1, -1, 0),
            new Vector3(-1, 1, 0),
            new Vector3(1, 1, 0),
        };

        static int vao, vbo, program;

        static string vs = @"#version 400

layout(location = 0) in vec3 vertex;

out vec2 tc;

void main()
{
	gl_Position = vec4(vertex, 1.0);
    tc = ((vertex.xy * vec2(1,-1)) + vec2(1)) * vec2(0.5);
}

";

        static string fs = @"#version 400

in vec2 tc;

uniform sampler2D textureSampler;

layout(location = 0) out vec4 RGBA;

void main(void)
{
    RGBA = texture2D(textureSampler, tc.xy).rgba;
}


";

        public static void DrawScreenFillingTexturedQuad()
        {
            if(vao == 0)
            {
                var v = GL.CreateShader(ShaderType.VertexShader);
                var f = GL.CreateShader(ShaderType.FragmentShader);
                program = GL.CreateProgram();

                GL.ShaderSource(v, vs);
                GL.ShaderSource(f, fs);

                GL.AttachShader(program, v);
                GL.AttachShader(program, f);
                GL.LinkProgram(program);

                Debug.WriteLine(GL.GetProgramInfoLog(program));

                vao = GL.GenVertexArray();
                int vbo = GL.GenBuffer();
                //int ebo = GL.GenBuffer();

                GL.BindVertexArray(vao);
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

                GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(vertices.Length * Vector3.SizeInBytes), vertices, BufferUsageHint.StaticDraw);

                int stride = BlittableValueType.StrideOf(vertices);

                GL.EnableVertexAttribArray(0);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
            }

            GL.UseProgram(program);
            GL.BindVertexArray(vao);
            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
            GL.BindVertexArray(0);
            GL.UseProgram(0);
        }
    }
}
