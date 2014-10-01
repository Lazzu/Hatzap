using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hatzap.Textures;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Hatzap.Utilities
{
    public class FrameBuffer
    {
        int fbo, depthBuffer, vao;
        List<Texture> targets = new List<Texture>();
        GameWindow gameWindow;

        public bool UseDepthBuffer { get; set; }
        public int BufferCount
        {
            get
            {
                return targets.Count;
            }
        }

        public FrameBuffer(GameWindow gw)
        {
            fbo = GL.GenFramebuffer();
            gameWindow = gw;
            UseDepthBuffer = true;
        }

        public void AddTarget(PixelInternalFormat format, PixelType type)
        {
            Texture target = new Texture();

			// Bind the texture
			target.Bind();

			// Give an empty image to OpenGL
			GL.TexImage2D(TextureTarget.Texture2D, 0, format, gameWindow.Width, gameWindow.Height, 0, PixelFormat.Rgba, type, IntPtr.Zero);

			// Poor filtering. Needed !
			GL.TexParameter(TextureTarget.Texture2D, 
				            TextureParameterName.TextureMinFilter, 
				            (int)TextureMagFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, 
				            TextureParameterName.TextureMagFilter, 
				            (int)TextureMagFilter.Nearest);

            targets.Add(target);
        }

        public void Create()
        {
            Activate();

            DrawBuffersEnum[] list = new DrawBuffersEnum[targets.Count];

            // Attach textures to the framebuffer and set the type in the list
            for (int i = 0; i < targets.Count; i++)
            {
                GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0 + i, targets[i].ID, 0);
                list[i] = DrawBuffersEnum.ColorAttachment0 + i;
            }

            if (UseDepthBuffer)
            {
                depthBuffer = GL.GenRenderbuffer();

                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthBuffer);
                GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer,
                                       RenderbufferStorage.DepthComponent32,
                                       gameWindow.Width, gameWindow.Height);

                // Attach the depth buffer to the framebuffer
                GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer,
                                           FramebufferAttachment.DepthAttachment,
                                           RenderbufferTarget.Renderbuffer,
                                           depthBuffer);
            }

            // Set the list of draw buffers.
            GL.DrawBuffers(list.Length, list);

            // Get error code
            FramebufferErrorCode status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);

            // Check for errors
            if (status != FramebufferErrorCode.FramebufferComplete)
                throw new Exception("Error creating frame buffer! Status: " + status);

            Deactivate();

            vao = GL.GenVertexArray();
            int vbo = GL.GenBuffer();
            int ebo = GL.GenBuffer();

            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);

            var vertices = new Vector3[] {
                new Vector3(-1, -1, 0),
                new Vector3(1, -1, 0),
                new Vector3(-1, 1, 0),
                new Vector3(1, 1, 0),
            };

            var indices = new int[] {
                0,1,2,
                1,3,2
            };

            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(vertices.Length * Vector3.SizeInBytes), vertices, BufferUsageHint.StaticDraw);
            GL.BufferData(BufferTarget.ElementArrayBuffer, new IntPtr(indices.Length * sizeof(int)), indices, BufferUsageHint.StaticDraw);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, BlittableValueType.StrideOf(vertices), 0);

            GL.BindVertexArray(0);
        }

        public void Activate()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fbo);
        }

        public void Deactivate()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void Render()
        {
            GL.BindVertexArray(vao);
            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }
    }
}
