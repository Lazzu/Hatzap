using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hatzap.Textures;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Hatzap.Rendering
{
    public class Framebuffer
    {
        int fbo = 0;
        Texture texture;
        VertexBatch batch;

        public Framebuffer(int width, int height)
        {
            texture = new Texture();
            texture.Quality = new TextureQuality();
            texture.Quality.TextureWrapMode = TextureWrapMode.ClampToEdge;
            texture.Quality.Filtering = TextureFiltering.Trilinear;
            texture.Generate(PixelFormat.Rgba, PixelType.UnsignedByte);

            texture.Bind();
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.GenerateMipmap, 1); // automatic mipmap
            texture.UnBind();

            // create a renderbuffer object to store depth info
            int rboId;
            GL.GenRenderbuffers(1, out rboId);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rboId);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent, width, height);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);

            // create a framebuffer object
            GL.GenFramebuffers(1, out fbo);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fbo);

            // attach the texture to FBO color attachment point
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer,        // 1. fbo target: GL_FRAMEBUFFER 
                                   FramebufferAttachment.ColorAttachment0,  // 2. attachment point
                                   TextureTarget.Texture2D,         // 3. tex target: GL_TEXTURE_2D
                                   texture.ID,             // 4. tex ID
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
            batch.Add(new Vector3(-1, 1, 0));  // Top left
            batch.Add(new Vector3(1, 1, 0)); // Top right
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

        public void RenderOnScreen()
        {
            texture.Bind();
            batch.Render();
            texture.UnBind();
        }
    }
}
