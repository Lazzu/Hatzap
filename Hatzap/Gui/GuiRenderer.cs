using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hatzap.Gui.Widgets;
using Hatzap.Shaders;
using Hatzap.Utilities;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Hatzap.Gui
{
    public class GuiRenderer
    {
        int vao = 0, vbo = 0, ebo = 0;

        int count = 0;

        ShaderProgram program;

        public GuiRenderer()
        {
            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();
            //ebo = GL.GenBuffer();

            var tmp = new GuiVertex[1];

            int stride = BlittableValueType.StrideOf(tmp);

            GL.BindVertexArray(vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, stride, 0);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, stride, Vector2.SizeInBytes);

            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 1, VertexAttribPointerType.UnsignedInt, false, stride, Vector2.SizeInBytes * 2);

            //GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);

            GL.BindVertexArray(0);
        }

        void RecursiveBuild(WidgetGroup rootGroup, List<GuiVertex> vertices)
        {
            foreach (var widget in rootGroup.Widgets)
            {
                var vert = widget.GetVertices();

                if (vert != null)
                    vertices.AddRange(vert);

                var group = widget as WidgetGroup;

                if (group != null)
                    RecursiveBuild(group, vertices);
            }
        }

        List<GuiVertex> vertices;

        public void Build(WidgetGroup rootGroup)
        {
            vertices = new List<GuiVertex>();

            RecursiveBuild(rootGroup, vertices);

            count = vertices.Count;
        }

        public void Render()
        {
            if(vertices != null)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
                GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(vertices.Count * GuiVertex.SizeInBytes), vertices.ToArray(), BufferUsageHint.DynamicDraw);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                vertices = null;
            }

            GL.BindVertexArray(vao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, count);
            GL.BindVertexArray(0);
        }
    }
}
