using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hatzap;
using Hatzap.Assets;
using Hatzap.Gui;
using Hatzap.Gui.Fonts;
using Hatzap.Models;
using Hatzap.Rendering;
using Hatzap.Shaders;
using Hatzap.Textures;
using Hatzap.Utilities;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace FramebufferExample
{
    class FramebufferExampleWindow : GameWindow
    {
        public FramebufferExampleWindow()
            : base(1280, 720, new GraphicsMode(new ColorFormat(32), 0, 0, 0, 0, 2, false), "Hatzap Framebuffer Example", GameWindowFlags.Default,
                DisplayDevice.GetDisplay(DisplayIndex.Default), 3, 3, GraphicsContextFlags.Default)
        {
            VSync = VSyncMode.Adaptive;
        }

        VertexBatch batch;
        ShaderProgram fboShader, blurShader, msaaShader;
        Framebuffer fbo;
        Model model;
        private Camera camera;
        private TextureManager textures;
        private RenderQueue renderQueue;

        // Effect toggles
        private bool blur = false;
        private bool msaa = false;
        private bool fxaa = false;
        private int msaaLevel = 32;
        private ShaderProgram textShader;
        private Font font;
        private GuiText text;
        private ShaderProgram fxaaShader;

        protected override void OnLoad(EventArgs e)
        {
            // Initialize GL settings
            GPUCapabilities.Initialize();
            GLState.DepthTest = true;
            GLState.CullFace = true;
            GLState.BlendFunc(BlendingFactorSrc.DstAlpha, BlendingFactorDest.OneMinusDstAlpha);

            PackageManager.BasePath = "../../Assets/";

            // Load shaders
            ShaderManager.LoadCollection("Shaders/collection.xml");
            fboShader = ShaderManager.Get("framebufferexample");
            blurShader = ShaderManager.Get("framebufferexample.blur");
            msaaShader = ShaderManager.Get("framebufferexample.msaa");
            fxaaShader = ShaderManager.Get("framebufferexample.fxaa");

            // Initialize framebuffer
            if(msaa)
            {
                fbo = new Framebuffer(Width, Height, 32);
            }
            else
            {
                fbo = new Framebuffer(Width, Height, 0);
            }

            

            // Load other stuff
            LoadMeshStuff();

            textShader = ShaderManager.Get("Text");

            font = new Font();
            font.LoadBMFont("Fonts/OpenSans-Regular.ttf_sdf.txt");
            font.Texture = textures.Get("Textures/OpenSans-Regular.ttf_sdf.tex", true);

            text = new GuiText();
            text.Font = font;
            text.Text = GenerateInfoText();
        }

        private string GenerateInfoText()
        {
            return string.Format("Options:\n(R)eset\n(B)lur: {0}\n(M)SAA: {1}\nMSAA Level: {2} (change with 1,2,3,4,5,6,7,8,9)\n(F)XAA: {3}\nFPS: {4}\nMS per frame: {5}", 
                blur, msaa, msaaLevel, fxaa, fps, msPerFrame);
        }

        void LoadMeshStuff()
        {
            // Set up camera
            camera = new Camera(this);
            camera.Perspective(Width, Height, (float)Math.PI / 4, 1f, 1000);
            camera.Position = new Vector3(0, 20, 200);
            camera.Target = new Vector3(0, 0, 0);
            camera.SetAsCurrent(); // Use this camera for rendering

            // Set up PackageManager
            PackageManager.BasePath = "../../Assets/";

            // Load shaders
            var modelshader = ShaderManager.Get("transparentmodel");

            // Load texture
            textures = new TextureManager();
            var texture = textures.Get("Textures/lucymetal.tex", true);
            texture.Quality.Mipmaps = true;

            // Load up a mesh
            MeshManager meshManager = new MeshManager();
            var mesh = meshManager.Get("Meshes/lucy.mesh", true);

            // Construct a model from shader, texture and mesh, with default material
            model = new Model()
            {
                Shader = modelshader,
                Texture = texture,
                Mesh = mesh,
                Material = new Material()
            };

            // set up rendering queue
            renderQueue = new RenderQueue()
            {
                AllowInstancing = false
            };
        }

        protected override void OnResize(EventArgs e)
        {
            // Update viewport and camera settings
            GL.Viewport(0, 0, Width, Height);
            camera.Perspective(Width, Height, (float)Math.PI / 4, 1f, 1000);

            // Release old and generate new framebuffer
            fbo.Release();
            if (msaa)
            {
                fbo = new Framebuffer(Width, Height, msaaLevel);
            }
            else
            {
                fbo = new Framebuffer(Width, Height, 0);
            }
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            // Rotate model around Y axis
            model.Transform.Rotation *= new Quaternion(0, (float)e.Time * 0.2f, 0);

            // Update camera
            camera.Update((float)e.Time);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            if (e.KeyChar == 'r')
            {
                if(msaa)
                {
                    fbo.Release();
                    fbo = new Framebuffer(Width, Height, 0);
                }
                blur = false;
                msaa = false;
                fxaa = false;
            }
            if(e.KeyChar == 'b')
            {
                if (msaa)
                {
                    fbo.Release();
                    fbo = new Framebuffer(Width, Height, 0);
                }
                blur = true;
                msaa = false;
                fxaa = false;
            }
            if (e.KeyChar == 'f')
            {
                if (msaa)
                {
                    fbo.Release();
                    fbo = new Framebuffer(Width, Height, 0);
                }
                blur = false;
                msaa = false;
                fxaa = true;
            }
            if (e.KeyChar == 'm')
            {
                if (!msaa)
                {
                    fbo.Release();
                    fbo = new Framebuffer(Width, Height, msaaLevel);
                }
                blur = false;
                msaa = true;
                fxaa = false;
            }
            if(msaa)
            {
                if (e.KeyChar == '1')
                {
                    fbo.Release();
                    msaaLevel = 1;
                    fbo = new Framebuffer(Width, Height, msaaLevel);
                    msaaLevel = fbo.MSAA;
                }
                if (e.KeyChar == '2')
                {
                    fbo.Release();
                    msaaLevel = 2;
                    fbo = new Framebuffer(Width, Height, msaaLevel);
                    msaaLevel = fbo.MSAA;
                }
                if (e.KeyChar == '3')
                {
                    fbo.Release();
                    msaaLevel = 4;
                    fbo = new Framebuffer(Width, Height, msaaLevel);
                    msaaLevel = fbo.MSAA;
                }
                if (e.KeyChar == '4')
                {
                    fbo.Release();
                    msaaLevel = 8;
                    fbo = new Framebuffer(Width, Height, msaaLevel);
                    msaaLevel = fbo.MSAA;
                }
                if (e.KeyChar == '5')
                {
                    fbo.Release();
                    msaaLevel = 16;
                    fbo = new Framebuffer(Width, Height, msaaLevel);
                    msaaLevel = fbo.MSAA;
                }
                if (e.KeyChar == '6')
                {
                    fbo.Release();
                    msaaLevel = 32;
                    fbo = new Framebuffer(Width, Height, msaaLevel);
                    msaaLevel = fbo.MSAA;
                }
                if (e.KeyChar == '7')
                {
                    fbo.Release();
                    msaaLevel = 64;
                    fbo = new Framebuffer(Width, Height, msaaLevel);
                    msaaLevel = fbo.MSAA;
                }
                if (e.KeyChar == '8')
                {
                    fbo.Release();
                    msaaLevel = 128;
                    fbo = new Framebuffer(Width, Height, msaaLevel);
                    msaaLevel = fbo.MSAA;
                }
                if (e.KeyChar == '9')
                {
                    fbo.Release();
                    msaaLevel = 256;
                    fbo = new Framebuffer(Width, Height, msaaLevel);
                    msaaLevel = fbo.MSAA;
                }
            }
            
        }

        double fps;
        double msPerFrame;

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            msPerFrame = Math.Round(1000.0 * e.Time, 2);
            fps = Math.Round(1.0 / e.Time, 2);

            // Clear the screen, just the color since we don't have depth in the OS framebuffer
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GLState.DepthTest = true;
            GLState.AlphaBleding = false;

            // Start rendering to framebuffer
            fbo.Enable();

            // Render stuff in fbo like you would normally

            // Clear the framebuffer
            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Render a model
            renderQueue.Insert(model);
            renderQueue.Render();

            // Stop rendering to framebuffer
            fbo.Disable();

            // No depth testing is needed at this moment (it may have been activated at some point when rendering to the fbo)
            GLState.DepthTest = false;
            GLState.AlphaBleding = false;

            // Set active shader for drawing
            ShaderProgram activeShader = fboShader;
            if(blur)
            {
                activeShader = blurShader;
                //activeShader.Enable();
            }
            else if (msaa)
            {
                activeShader = msaaShader;
                //activeShader.Enable();
            } 
            else if(fxaa)
            {
                activeShader = fxaaShader;
                activeShader.Enable();
                activeShader.SendUniform("R_inverseFilterTextureSize", new Vector3(1.0f / Width, 1.0f / Height, 0));
                activeShader.SendUniform("R_fxaaSpanMax", 8.0f);
                activeShader.SendUniform("R_fxaaReduceMin", 1.0f / 128.0f );
                activeShader.SendUniform("R_fxaaReduceMul", 1.0f / 8.0f );
            }

            // Fill screen with FBO using some shader.
            fbo.RenderOnScreen(activeShader);

            DrawText();

            // Display rendered frame
            SwapBuffers();
        }

        private void DrawText()
        {
            text.Text = GenerateInfoText();
            textShader.Enable();
            GLState.DepthTest = false;
            GLState.AlphaBleding = true;
            GLState.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            Matrix4 projection = Matrix4.CreateOrthographicOffCenter(0, Width, Height, 0, -1, 1);
            Matrix4 view = Matrix4.CreateTranslation(0, 20, 0);
            var textureSize = new Vector2(text.Font.Texture.Width, text.Font.Texture.Height);
            var mvp = view * projection;
            textShader.SendUniform("MVP", ref mvp);
            textShader.SendUniform("textureSize", ref textureSize);
            GL.ActiveTexture(TextureUnit.Texture0);

            text.Color = new Vector4(1, 1, 1, 1);

            // Set font size
            text.FontSize = 10.0f;

            text.Smooth = .9f;
            text.Weight = 1.5f;
            text.LineHeight = 100f;
            // Draw the text
            text.Draw(textShader);

            textShader.Disable();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);

            AssetManagerBase.CleanupManagers();
            fbo.Release();
        }
    }
}
