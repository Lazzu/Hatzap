using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hatzap.Shaders;
using Hatzap.Utilities;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Hatzap.Sprites
{
    public class SpriteBatch
    {
        List<SpriteRenderData> queue = new List<SpriteRenderData>();
        List<uint> indices = new List<uint>();

        SpriteAtlas current = null;

        int[] vbo = new int[2];
        int vao;
        ShaderProgram program;

        public bool PremultipliedSprites { get; set; }

        public void BeginBatch(SpriteAtlas atlas)
        {
            current = atlas;

            if (vao == 0)
            {
                program = ShaderManager.Get("sprite.shader");
                
                vao = GL.GenVertexArray();
                GL.GenBuffers(2, vbo);

                GL.BindVertexArray(vao);
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo[0]);

                int vertex = program.GetAttribLocation("vertex");
                int uv = program.GetAttribLocation("uv");
                int position = program.GetAttribLocation("position");
                int size = program.GetAttribLocation("size");
                int rotation = program.GetAttribLocation("rotation");
                int color = program.GetAttribLocation("color");

                int stride = BlittableValueType.StrideOf(new SpriteRenderData());

                int offset = 0;

                GL.EnableVertexAttribArray(vertex);
                GL.VertexAttribPointer(vertex, 3, VertexAttribPointerType.Float, false, stride, offset);

                offset += Vector3.SizeInBytes;

                if(uv != -1)
                {
                    GL.EnableVertexAttribArray(uv);
                    GL.VertexAttribPointer(uv, 2, VertexAttribPointerType.Float, false, stride, offset);
                }

                offset += Vector2.SizeInBytes;

                if(position != -1)
                {
                    GL.EnableVertexAttribArray(position);
                    GL.VertexAttribPointer(position, 3, VertexAttribPointerType.Float, false, stride, offset);
                }

                offset += Vector3.SizeInBytes;

                if(size != -1)
                {
                    GL.EnableVertexAttribArray(size);
                    GL.VertexAttribPointer(size, 2, VertexAttribPointerType.Float, false, stride, offset);
                }

                offset += Vector2.SizeInBytes;

                if(rotation != -1)
                {
                    GL.EnableVertexAttribArray(rotation);
                    GL.VertexAttribPointer(rotation, 1, VertexAttribPointerType.Float, false, stride, offset);
                }

                offset += sizeof(float);

                if (color != -1)
                {
                    GL.EnableVertexAttribArray(color);
                    GL.VertexAttribPointer(color, 4, VertexAttribPointerType.Float, false, stride, offset);
                }

                offset += Vector4.SizeInBytes;
                

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, vbo[1]);

                GL.BindVertexArray(0);
            }
        }

        public void EndBatch()
        {
            Render();

            current = null;
        }

        private void Render()
        {
            var data = queue.ToArray();
            var index = indices.ToArray();

            queue.Clear();
            indices.Clear();

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo[0]);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(data.Length * SpriteRenderData.SizeInBytes), data, BufferUsageHint.StreamDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, vbo[1]);
            GL.BufferData(BufferTarget.ElementArrayBuffer, new IntPtr(index.Length * sizeof(uint)), index, BufferUsageHint.StreamDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            GLState.AlphaBleding = true;

            if(current.PremultipliedAlpha)
            {
                GLState.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcAlpha);
            }
            else
            {
                GLState.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            }

            program.Enable();
            program.SendUniform("projection", ref Camera.Current.Projection);
            current.Atlas.Bind();
            current.Atlas.UpdateQuality();

            GL.Enable(EnableCap.PrimitiveRestartFixedIndex);
            GL.PrimitiveRestartIndex(int.MaxValue - 1);
            GL.BindVertexArray(vao);
            GL.DrawRangeElements(PrimitiveType.Triangles, 0, index.Length - 1, index.Length, DrawElementsType.UnsignedInt, IntPtr.Zero);
            GL.BindVertexArray(0);

            program.Disable();
            current.Atlas.UnBind();
        }

        public void Draw(Sprite sprite, Vector2 position, Vector2 size, float rotation, Vector4 color)
        {
            /*if(current.PremultipliedAlpha)
            {
                color.X *= color.W;
                color.Y *= color.W;
                color.Z *= color.W;
            }*/

            uint baseIndex = (uint)queue.Count;

            for (int i = 0; i < sprite.Vertices.Length; i++)
            {
                queue.Add(new SpriteRenderData()
                {
                    Vertex = sprite.Vertices[i],
                    UV = sprite.TextureCoordinates[i],
                    Position = new Vector3(position),
                    Size = size * sprite.Size,
                    Rotation = rotation,
                    Color = color
                });
            }

            for (int i = 0; i < sprite.Indices.Length; i++)
            {
                indices.Add(baseIndex + sprite.Indices[i]);
            }
        }
    }
}
