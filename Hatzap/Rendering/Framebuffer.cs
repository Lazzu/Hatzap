using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hatzap.Shaders;
using Hatzap.Textures;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Hatzap.Rendering
{
    public class Framebuffer
    {
        int fbo = 0;
        int rboId;
        public Texture Texture { get; protected set; }
        VertexBatch batch;
        private Vector2 size;

        public int Width { get; protected set; }

        public int Height { get; protected set; }

        public int MSAA { get; protected set; }

        public int MSAACoverage { get; protected set; }

        public Framebuffer(int width, int height, int msaa)
        {
            Width = width;
            Height = height;
            MSAA = msaa;
            size = new Vector2(width, height);

            Initialize();
        }

        void Initialize()
        {
            var textureQuality = new TextureQuality()
            {
                TextureWrapMode = TextureWrapMode.ClampToEdge,
                Filtering = TextureFiltering.Nearest,
            };

            TextureTarget target;

            int texID = GL.GenTexture();

            var msaa = GL.GetInteger(GetPName.MaxSamples);

            if (MSAA < 0)
                MSAA = 0;

            if (MSAA > msaa)
                MSAA = msaa;

            switch(MSAA)
            {
                case 2:
                case 4:
                case 8:
                case 16:
                case 32:
                case 64:
                case 128:
                    target = TextureTarget.Texture2DMultisample;

                    GL.BindTexture(target, texID);
                    GL.TexImage2DMultisample(TextureTargetMultisample.Texture2DMultisample, MSAA, PixelInternalFormat.Rgba, Width, Height, true);

                    break;
                default:
                    target = TextureTarget.Texture2D;

                    GL.BindTexture(target, texID);
                    GL.TexImage2D(target, 0, PixelInternalFormat.Rgba, Width, Height, 0, PixelFormat.Rgba, PixelType.Byte, IntPtr.Zero);

                    break;
            }

            GL.BindTexture(target, 0);

            Texture = new Texture(texID, Width, Height, target, PixelInternalFormat.Rgba);
            Texture.Quality = textureQuality;

            Texture.Bind();
            Texture.UpdateQuality();
            Texture.UnBind();

            /*texture.Bind();
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.GenerateMipmap, 1); // automatic mipmap
            texture.UnBind();*/

            // create a renderbuffer object to store depth info
            GL.GenRenderbuffers(1, out rboId);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rboId);

            if(MSAA > 0)
            {
                GL.RenderbufferStorageMultisample(RenderbufferTarget.Renderbuffer, MSAA, RenderbufferStorage.DepthComponent32, Width, Height);
            }
            else
            {
                GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent32, Width, Height);
            }

            
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);

            // create a framebuffer object
            GL.GenFramebuffers(1, out fbo);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fbo);

            // attach the texture to FBO color attachment point
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer,        // 1. fbo target: GL_FRAMEBUFFER 
                                   FramebufferAttachment.ColorAttachment0,  // 2. attachment point
                                   Texture.TextureTarget,         // 3. tex target: GL_TEXTURE_2D
                                   Texture.ID,             // 4. tex ID
                                   0);                    // 5. mipmap level: 0(base)

            // attach the renderbuffer to depth attachment point
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer,      // 1. fbo target: GL_FRAMEBUFFER
                                      FramebufferAttachment.DepthAttachment, // 2. attachment point
                                      RenderbufferTarget.Renderbuffer,     // 3. rbo target: GL_RENDERBUFFER
                                      rboId);              // 4. rbo ID

            // check FBO status
            var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (status != FramebufferErrorCode.FramebufferComplete)
                Console.WriteLine("ERROR: Can not create framebuffer because " + status.ToString());

            // switch back to window-system-provided framebuffer
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            batch = new VertexBatch();
            batch.StartBatch(PrimitiveType.Triangles);
            batch.Add(new Vector3(-1, -1, 0)); // Bottom left
            batch.Add(new Vector3(1, -1, 0));  // Bottom right
            batch.Add(new Vector3(-1, 1, 0));  // Top left
            batch.Add(new Vector3(1, -1, 0));  // Bottom right
            batch.Add(new Vector3(1, 1, 0)); // Top right
            batch.Add(new Vector3(-1, 1, 0));  // Top left
            batch.EndBatch();
        }

        public void Enable()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fbo);
        }

        public void Disable()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void RenderOnScreen(ShaderProgram shader)
        {
            shader.Enable();
            shader.SendUniform("ScreenSize", ref size);
            shader.SendUniform("MSAA_Samples", MSAA);
            Texture.Bind();
            batch.Render();
            Texture.UnBind();
            shader.Disable();
        }

        public void Release()
        {
            GL.DeleteFramebuffer(fbo);
            GL.DeleteRenderbuffer(rboId);
            if(Texture != null) Texture.Release();
            Texture = null;
        }
    }
}
