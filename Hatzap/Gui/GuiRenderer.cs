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
        int vao = 0, vbo = 0, uvVbo = 0, colorVbo = 0, ebo = 0;

        int count = 0;

        ShaderProgram program;

        public GuiRenderer()
        {
            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();
            uvVbo = GL.GenBuffer();
            colorVbo = GL.GenBuffer();
            //ebo = GL.GenBuffer();

            var tmp = new Vector2[1];
            var tmp2 = new Vector4[1];
            var tmp3 = new Vector3[1];

            int stride = BlittableValueType.StrideOf(tmp);
            int stride2 = BlittableValueType.StrideOf(tmp2);
            int stride3 = BlittableValueType.StrideOf(tmp3);

            GL.BindVertexArray(vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, colorVbo);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, stride2, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, stride, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, uvVbo);
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, stride3, 0);

            //GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);

            GL.BindVertexArray(0);
        }

        void RecursiveBuild(WidgetGroup rootGroup, List<Vector2> vertices, List<Vector3> uv, List<Vector4> colors, ref int index)
        {
            foreach (var widget in rootGroup.Widgets)
            {
                if (!widget.Visible)
                    continue;

                if(widget.Anchor != null)
                    widget.Anchor.SetCoordinates(widget.position, widget.size);

                widget.Dirty = false;

                int start = vertices.Count;

                widget.GetVertices(vertices, uv, colors);

                int current = vertices.Count;

                if (vertices != null && vertices.Count > 0 && start < current)
                {
                    index += current - start;

                    widget.drawStartIndex = start;
                    widget.drawEndIndex = current;
                }

                var group = widget as WidgetGroup;

                if (group != null)
                    RecursiveBuild(group, vertices, uv, colors, ref index);
            }
        }

        List<Vector2> vertices;
        List<Vector3> uv;
        List<Vector4> colors;
        bool verticesUpdated = false, colorsUpdated = false;

        public void Build(WidgetGroup rootGroup)
        {
            vertices = new List<Vector2>();
            uv = new List<Vector3>();
            colors = new List<Vector4>();

            Debug.WriteLine("ReBuilding GUI");

            lock(vertices)
            {
                int index = 0;

                RecursiveBuild(rootGroup, vertices, uv, colors, ref index);

                rootGroup.Dirty = false;

                count = vertices.Count;
                reupload = true;
            }
        }

        bool reupload = true;

        public void Render()
        {
            //Debug.WriteLine("Reupload: " + reupload + " Vertices: " + (vertices != null) + " Colors: " + colorsUpdated);

            if(reupload && vertices != null)
            {
                Vector2[] vert = null;
                Vector3[] uvs = null;
                Vector4[] colrs = null;

                lock (vertices)
                {
                    vert = vertices.ToArray();
                }

                lock (colors)
                {
                    colrs = colors.ToArray();
                }

                lock (uv)
                {
                    uvs = uv.ToArray();
                }

                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
                GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(vertices.Count * Vector2.SizeInBytes), vert, BufferUsageHint.DynamicDraw);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                GL.BindBuffer(BufferTarget.ArrayBuffer, uvVbo);
                GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(uv.Count * Vector3.SizeInBytes), uvs, BufferUsageHint.DynamicDraw);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                GL.BindBuffer(BufferTarget.ArrayBuffer, colorVbo);
                GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(colors.Count * Vector4.SizeInBytes), colrs, BufferUsageHint.DynamicDraw);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                reupload = false;
            }
            else
            {
                if (colorsUpdated)
                {
                    UpdateColorBuffer();
                    colorsUpdated = false;
                }
            }
            

            GL.BindVertexArray(vao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, count);
            GL.BindVertexArray(0);
        }

        int colorStartIndex = int.MaxValue, colorLastIndex = int.MinValue;

        internal void UpdateColor(int startIndex, int lastIndex, Vector4 color)
        {
            if (colors == null)
                return;

            colorsUpdated = true;

            if (colorStartIndex > startIndex)
                colorStartIndex = startIndex;

            if (colorLastIndex < lastIndex)
                colorLastIndex = lastIndex;

            for(int i = startIndex; i <= lastIndex; i++)
            {
                colors[i] = color;
            }
        }

        void UpdateColorBuffer()
        {
            int size = colorLastIndex - colorStartIndex;
            var colors = this.colors.GetRange(colorStartIndex, size).ToArray();
            
            GL.BindBuffer(BufferTarget.ArrayBuffer, colorVbo);
            GL.BufferSubData(BufferTarget.ArrayBuffer, new IntPtr(colorStartIndex * Vector4.SizeInBytes), new IntPtr(size * Vector4.SizeInBytes), colors);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            colorsUpdated = false;
            colorStartIndex = int.MaxValue;
            colorLastIndex = int.MinValue;
        }
    }
}
