using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Hatzap.Rendering
{

    /// <summary>
    /// VertexBatch is used to render simple vertices on screen.
    /// </summary>
    public class VertexBatch
    {
        public PrimitiveType Type { get; protected set; }

        List<Vector3> vertices = new List<Vector3>();
        List<int> indices = new List<int>();

        int vbo = 0, ebo = 0, vao = 0;

        public void StartBatch(PrimitiveType type)
        {
            if(vbo == 0)
            {
                vbo = GL.GenBuffer();
                ebo = GL.GenBuffer();
                vao = GL.GenVertexArray();

                GL.BindVertexArray(vao);
                GL.EnableVertexAttribArray(0);
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
                GL.BindVertexArray(0);
            }

            Type = type;
            vertices.Clear();
        }

        public void EndBatch()
        {
            int count = vertices.Count;

            switch(Type)
            {
                case PrimitiveType.Triangles:
                case PrimitiveType.TriangleFan:
                case PrimitiveType.TriangleStrip:
                    if (count < 3) throw new InvalidOperationException( Type.ToString() + " must have at least three vertices!");
                    break;
                case PrimitiveType.Lines:
                case PrimitiveType.LineStrip:
                case PrimitiveType.LineLoop:
                    if (count < 2) throw new InvalidOperationException(Type.ToString() + " must have at least two vertices!");
                    break;
                case PrimitiveType.Points:
                    if (count < 1) throw new InvalidOperationException(Type.ToString() + " must have at least one vertex!");
                    break;
            }

            var vert = vertices.ToArray();

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(vertices.Count * Vector3.SizeInBytes), vert, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, new IntPtr(indices.Count * sizeof(int)), indices.ToArray(), BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        public void Add(Vector3 vertex)
        {
            indices.Add(vertices.Count);
            vertices.Add(vertex);
        }

        public void Add(IEnumerable<Vector3> v)
        {
            vertices.AddRange(v);
        }

        public void Render()
        {
            GL.BindVertexArray(vao);
            GL.DrawElements(Type, vertices.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);
            GL.BindVertexArray(0);
        }
    }
}
