using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Hatzap.Models;
using Hatzap.Shaders;
using Hatzap.Utilities;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Hatzap.Rendering
{
    public class InstancedBatch
    {
        public Mesh Mesh { get; protected set; }

        public ShaderProgram Shader { get; protected set; }

        public int Count { get; protected set; }

        Dictionary<Material, List<Matrix4>> matrices = new Dictionary<Material,List<Matrix4>>();
        Dictionary<Material, int> mbo = new Dictionary<Material,int>();

        int pos1, pos2, pos3, pos4, stride, attribPos, byteSize, c;

        // Statics to improve performance a little bit
        static Material material;
        static List<Matrix4> matrix;

        public InstancedBatch(Model model)
        {
            Mesh = model.Mesh;
            Shader = model.Shader;
            Vector4[] tmp = null;
            stride = BlittableValueType.StrideOf(tmp) * 4;
            attribPos = Shader.GetAttribLocation("mInstancedModelMatrix");
            pos1 = attribPos + 0;
            pos2 = attribPos + 1;
            pos3 = attribPos + 2;
            pos4 = attribPos + 3;
        }

        public void Insert(Renderable obj)
        {
            if(!matrices.TryGetValue(obj.Material, out matrix))
            {
                matrix = new List<Matrix4>();
                matrices.Add(obj.Material, matrix);

                if(!mbo.ContainsKey(obj.Material)) 
                    mbo.Add(obj.Material, GL.GenBuffer());
            }

            matrix.Add(obj.Transform.Matrix);
            Count++;
        }

        bool first = true;

        public int Draw()
        {
            Time.StartTimer("InstancedBatch.Draw()", "Render");

            Mesh.VertexAttribLocation = Shader.GetAttribLocation("vertex");
            Mesh.NormalAttribLocation = Shader.GetAttribLocation("normal");
            Mesh.TangentAttribLocation = Shader.GetAttribLocation("tangent");
            Mesh.BinormalAttribLocation = Shader.GetAttribLocation("binormal");
            Mesh.UVAttribLocation = Shader.GetAttribLocation("uv");
            Mesh.ColorAttribLocation = Shader.GetAttribLocation("color");

            Mesh.Bind();

            c = 0;

            foreach (var materialGroup in matrices)
            {
                material = materialGroup.Key;
                matrix = materialGroup.Value;

                GL.BindBuffer(BufferTarget.ArrayBuffer, mbo[material]);
                GL.VertexAttribPointer(pos1, 4, VertexAttribPointerType.Float, false, stride, Vector4.SizeInBytes * 0);
                GL.VertexAttribPointer(pos2, 4, VertexAttribPointerType.Float, false, stride, Vector4.SizeInBytes * 1);
                GL.VertexAttribPointer(pos3, 4, VertexAttribPointerType.Float, false, stride, Vector4.SizeInBytes * 2);
                GL.VertexAttribPointer(pos4, 4, VertexAttribPointerType.Float, false, stride, Vector4.SizeInBytes * 3);
                GL.EnableVertexAttribArray(pos1);
                GL.EnableVertexAttribArray(pos2);
                GL.EnableVertexAttribArray(pos3);
                GL.EnableVertexAttribArray(pos4);
                GL.VertexAttribDivisor(pos1, 1);
                GL.VertexAttribDivisor(pos2, 1);
                GL.VertexAttribDivisor(pos3, 1);
                GL.VertexAttribDivisor(pos4, 1);

                byteSize = matrix.Count * Vector4.SizeInBytes * 4;

                GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(byteSize), matrix.ToArray(), BufferUsageHint.StreamDraw);

                /*GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(byteSize), IntPtr.Zero, BufferUsageHint.StreamDraw);

                try
                {
                    var buffer = GL.MapBuffer(BufferTarget.ArrayBuffer, BufferAccess.WriteOnly);

                    unsafe
                    {
                        var ptr = (Matrix4*)buffer.ToPointer();

                        for (int i = 0; i < matrix.Count; i++)
                        {
                            ptr[i] = matrix[i];
                        }
                    }
                    GL.UnmapBuffer(BufferTarget.ArrayBuffer);
                }
                catch(Exception e) 
                {
                    GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(byteSize), matrix.ToArray(), BufferUsageHint.StreamDraw);
                    Debug.WriteLine(e);
                }*/

                foreach (var uniform in material.Uniforms)
                {
                    uniform.SendData(Shader);
                }

                Mesh.DrawInstanced(matrix.Count);

                c += matrix.Count;

                matrix.Clear();
            }

            

            Mesh.UnBind();

            

            //matrices.Clear();
            Count = 0;

            Time.StopTimer("InstancedBatch.Draw()");

            return c * Mesh.Triangles;
        }
    }
}
