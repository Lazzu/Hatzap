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
        int vao = 0, vbo = 0, colorVbo = 0, ebo = 0;

        int count = 0;

        ShaderProgram program;

        public GuiRenderer()
        {
            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();
            colorVbo = GL.GenBuffer();
            //ebo = GL.GenBuffer();

            var tmp = new GuiVertex[1];
            var tmp2 = new Vector4[1];

            int stride = BlittableValueType.StrideOf(tmp);
            int stride2 = BlittableValueType.StrideOf(tmp2);

            GL.BindVertexArray(vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, colorVbo);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, stride2, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, stride, 0);

            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, stride, Vector2.SizeInBytes);

            GL.EnableVertexAttribArray(3);
            GL.VertexAttribPointer(3, 1, VertexAttribPointerType.UnsignedInt, false, stride, Vector2.SizeInBytes * 2);

            //GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);

            GL.BindVertexArray(0);
        }

        void RecursiveBuild(WidgetGroup rootGroup, List<GuiVertex> vertices, List<Vector4> colors, ref int index)
        {
            foreach (var widget in rootGroup.Widgets)
            {
                if (!widget.Visible)
                    continue;

                if(widget.Anchor != null)
                    widget.Anchor.SetCoordinates(widget.position, widget.size);

                widget.Dirty = false;

                var vert = widget.GetVertices();

                if (vert != null)
                {
                    int start = index;
                    index += vert.Length;

                    vertices.AddRange(vert);

                    for (int i = 0; i < vert.Length; i++)
                    {
                        colors.Add(widget.Color);
                    }
                }

                var group = widget as WidgetGroup;

                if (group != null)
                    RecursiveBuild(group, vertices, colors, ref index);
            }
        }

        List<GuiVertex> vertices;
        List<Vector4> colors;
        bool colorsUpdated = false;

        public void Build(WidgetGroup rootGroup)
        {
            vertices = new List<GuiVertex>();
            colors = new List<Vector4>();

            int index = 0;

            RecursiveBuild(rootGroup, vertices, colors, ref index);

            colorsUpdated = true;

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

            if (colorsUpdated)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, colorVbo);
                GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(colors.Count * Vector4.SizeInBytes), colors.ToArray(), BufferUsageHint.DynamicDraw);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                colorsUpdated = false;
            }

            GL.BindVertexArray(vao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, count);
            GL.BindVertexArray(0);
        }

        internal void UpdateColor(int startIndex, int lastIndex, Vector4 color)
        {
            colorsUpdated = true;

            for(int i = startIndex; i <= lastIndex; i++)
            {
                colors[i] = color;
            }

        }
    }
}
