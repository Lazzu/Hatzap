using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hatzap.Textures;
using OpenTK.Graphics.OpenGL;

namespace Hatzap.Rendering
{
    public class Framebuffer
    {
        int fbo = 0;

        public bool HasDepth { get; set; }

        public Framebuffer(int depth, int stencil, Texture[] renderTargets)
        {
            fbo = GL.GenFramebuffer();

            /*GL.BindFramebuffer(FramebufferTarget.Framebuffer, fbo);

            if(depth > 0)
            {
                // Depth buffer
                GL.GenRenderbuffers(1, out depthBuffer);
            }
            

            // Generate textures to render to
            GL.GenTextures(textures.Length, textures);

            // Create new textures and buffers
            ResizeTextures(size);

            // Create orthogonal projection
            ResizeOrtho(size);

            // Draw buffer types list
            DrawBuffersEnum[] list = new DrawBuffersEnum[textures.Length];

            // Attach textures to the framebuffer and set the type in the list
            for (int i = 0; i < textures.Length; i++)
            {
                GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0 + i, textures[i], 0);
                list[i] = DrawBuffersEnum.ColorAttachment0 + i;
            }

            // Attach the depth buffer to the framebuffer
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer,
                FramebufferAttachment.DepthAttachment,
                RenderbufferTarget.Renderbuffer,
                depthBuffer);

            // Set the list of draw buffers.
            GL.DrawBuffers(textures.Length, list);

            // Get error code
            FramebufferErrorCode status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);

            // Check for errors
            if (status != FramebufferErrorCode.FramebufferComplete)
                throw new Exception("Error creating frame buffer! Status: " + status);

            // Unbind the framebuffer
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            // Vertex buffer object
            GL.GenBuffers(1, out vbo);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertex.Length * Vector3.SizeInBytes), vertex, BufferUsageHint.DynamicDraw);

            // Element array object (indices)
            GL.GenBuffers(1, out ebo);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(index.Length * sizeof(ushort)), index, BufferUsageHint.StaticDraw);

            // Vertex array object (store vertex buffer and incides in same place)
            GL.GenVertexArrays(1, out vao);

            // bind the vao
            GL.BindVertexArray(vao);

            // Set the vbo settings in the vao
            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, BlittableValueType.StrideOf(vertex), 0);

            // Bind the ebo to the vao
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);

            // Unbind the vao
            GL.BindVertexArray(0);

            ResizeVertices(size);


            // Make the shader
            shader = new ShaderProgram();
            shader.ProcessShaderFile("Content/Shaders/Deferred/deferred.vert", ShaderType.VertexShader);
            shader.ProcessShaderFile("Content/Shaders/Deferred/deferred.frag", ShaderType.FragmentShader);
            shader.Link();


            // Make the debug shader
            debugShader = new ShaderProgram();
            debugShader.ProcessShaderFile("Content/Shaders/texture.vert", ShaderType.VertexShader);
            debugShader.ProcessShaderFile("Content/Shaders/texture.frag", ShaderType.FragmentShader);
            debugShader.Link();

            // Texture locations for deferred rendering
            shader.FindUniforms(textureLocations);

            shader.FindUniforms(new string[] {
				"mP", "DiffLightTexture", "SpecLightTexture", "brightness"
			});

            debugShader.FindUniforms(new string[]{
				"mP", "textureSampler"
			});*/
        }
    }
}
