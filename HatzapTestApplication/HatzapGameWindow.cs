using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Assimp;
using Assimp.Configs;
using Hatzap;
using Hatzap.Gui;
using Hatzap.Gui.Fonts;
using Hatzap.Input;
using Hatzap.Models;
using Hatzap.Shaders;
using Hatzap.Textures;
using Hatzap.Utilities;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace HatzapTestApplication
{
    class HatzapGameWindow : GameWindow
    {
        Hatzap.Camera camera;
        ShaderProgram skyShader;
        ShaderProgram modelShader;
        ShaderProgram textShader;

        Vector2 viewPort;

        Hatzap.Models.Mesh mesh;

        Texture texture;

        Hatzap.Gui.Fonts.Font font;
        GuiText text;
        GuiText boldText;
        GuiText largeText;
        GuiText fpsText;

        public HatzapGameWindow() : base(1280,720, new GraphicsMode(new ColorFormat(32), 32, 32, 8, new ColorFormat(8, 8, 8, 8), 2, false), "Hatzap Test Application", GameWindowFlags.Default, 
            DisplayDevice.GetDisplay(DisplayIndex.Default), 3, 3, GraphicsContextFlags.ForwardCompatible)
        {
            //WindowState = OpenTK.WindowState.Maximized;
        }

        protected override void OnLoad(EventArgs e)
        {
            Debug.WriteLine("OnLoad()");

            base.OnLoad(e);

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.VertexProgramPointSize);
            GL.Enable(EnableCap.CullFace);
            //GL.Enable(EnableCap.ScissorTest);

            viewPort = new Vector2(Width, Height);

            camera = new Hatzap.Camera(this);
            
            camera.SetAsCurrent();

            camera.Position = new Vector3(0, 0, 1.5f);
            camera.Target = new Vector3(0, 0, 0);

            Time.Initialize();
            UserInput.Initialize(this, typeof(AccurateMouse), typeof(Keyboard));
            GLThreadHelper.Initialize(this);
            GuiRoot.Initialize(this);

            UserInput.Keyboard.CaptureText = true;

            /*Shader skyVertex = new Shader(ShaderType.VertexShader);
            Shader skyFragment = new Shader(ShaderType.FragmentShader);

            using (StreamReader r = new StreamReader("Assets/Shaders/Sky.vert"))
            {
                skyVertex.ShaderSource(r.ReadToEnd());
            }

            using (StreamReader r = new StreamReader("Assets/Shaders/Sky.frag"))
            {
                skyFragment.ShaderSource(r.ReadToEnd());
            }

            skyShader = new ShaderProgram("Sky");
            skyShader.AttachShader(skyVertex);
            skyShader.AttachShader(skyFragment);
            skyShader.Link();
            skyShader.Enable();*/

            Shader textVertexShader = new Shader(ShaderType.VertexShader);
            Shader textFragmentShader = new Shader(ShaderType.FragmentShader);

            using (StreamReader r = new StreamReader("Assets/Shaders/Text.vert"))
            {
                textVertexShader.ShaderSource(r.ReadToEnd());
            }

            using (StreamReader r = new StreamReader("Assets/Shaders/Text.frag"))
            {
                textFragmentShader.ShaderSource(r.ReadToEnd());
            }

            textShader = new ShaderProgram("Text");
            textShader.AttachShader(textVertexShader);
            textShader.AttachShader(textFragmentShader);
            textShader.Link();
            textShader.Enable();

            Shader modelVertexShader = new Shader(ShaderType.VertexShader);
            Shader modelFragmentShader = new Shader(ShaderType.FragmentShader);

            using (StreamReader r = new StreamReader("Assets/Shaders/Model.vert"))
            {
                modelVertexShader.ShaderSource(r.ReadToEnd());
            }

            using (StreamReader r = new StreamReader("Assets/Shaders/Model.frag"))
            {
                modelFragmentShader.ShaderSource(r.ReadToEnd());
            }

            modelShader = new ShaderProgram("Model");
            modelShader.AttachShader(modelVertexShader);
            modelShader.AttachShader(modelFragmentShader);
            modelShader.Link();
            modelShader.Enable();

            //Create a new importer
            AssimpContext importer = new AssimpContext();

            //This is how we add a configuration (each config is its own class)
            //NormalSmoothingAngleConfig config = new NormalSmoothingAngleConfig(66.0f);
            //importer.SetConfig(config);

            //This is how we add a logging callback 
            /*LogStream logstream = new LogStream(delegate(String msg, String userData)
            {
                Debug.WriteLine(msg);
            });
            logstream.Attach();*/

            var flags = PostProcessPreset.TargetRealTimeMaximumQuality | PostProcessSteps.Triangulate | PostProcessSteps.SortByPrimitiveType | PostProcessSteps.FlipUVs;

            /*if ((flags & PostProcessSteps.Triangulate) == PostProcessSteps.Triangulate)
                flags ^= PostProcessSteps.Triangulate;*/

            //Import the model. All configs are set. The model
            //is imported, loaded into managed memory. Then the unmanaged memory is released, and everything is reset.
            Scene model = importer.ImportFile("Assets/Models/Sample_Ship.fbx", flags);
            
            mesh = new Hatzap.Models.Mesh();

            Assimp.Mesh aMesh = null;

            foreach (var item in model.Meshes)
            {
                if (item.PrimitiveType != Assimp.PrimitiveType.Triangle)
                    continue;

                aMesh = item;
                break;
            }

            if (aMesh != null)
            {
                mesh.AssimpMesh = aMesh;
            }
            else
            {
                Debug.WriteLine("ERROR: No triangle meshes found in imported model.");
            }

            //End of example
            importer.Dispose();

            texture = new Texture();
            texture.Load(new Bitmap("Assets/Textures/sh3.jpg"), PixelFormat.Bgra, PixelType.UnsignedByte);
            texture.Bind();
            texture.TextureSettings(TextureMinFilter.Nearest, TextureMagFilter.Nearest, 0);
            texture.GenMipMaps();
            texture.TextureSettings(TextureMinFilter.LinearMipmapLinear, TextureMagFilter.Linear, 32);

            font = new Hatzap.Gui.Fonts.Font();
            font.LoadBMFont("Assets/Fonts/OpenSans-Regular.ttf_sdf.txt");
            font.Texture = new Texture();
            font.Texture.Load(new Bitmap("Assets/Fonts/OpenSans-Regular.ttf_sdf.png"), PixelFormat.Bgra, PixelType.UnsignedByte);
            font.Texture.Bind();
            //font.Texture.TextureSettings(TextureMinFilter.Linear, TextureMagFilter.Linear, 32);
            //font.Texture.GenMipMaps();
            font.Texture.TextureSettings(TextureMinFilter.Linear, TextureMagFilter.Linear, 0);
            //font.Texture = texture;

            text = new GuiText();
            text.Font = font;
            text.FontSize = 10f;
            text.Weight = 1f;
            text.Smooth = 0.5f;
            text.Border = 0;
            text.Text = "\"The quick brown fox jumps over the lazy dog\" is an English-language pangram—a phrase that contains all of the letters of the alphabet.";

            boldText = new GuiText();
            boldText.Font = font;
            boldText.FontSize = 10f;
            boldText.Weight = 1.0f;
            boldText.Smooth = 0.5f;
            boldText.LineHeight = 50f;
            boldText.Text = "\"The quick brown fox jumps over the lazy dog\" is an English-language pangram—a phrase that contains all of the letters of the alphabet.";

            largeText = new GuiText();
            largeText.Font = font;
            largeText.FontSize = 30f;
            largeText.Weight = 1f;
            largeText.Smooth = 0.25f;
            largeText.Border = 0.5f;
            largeText.LineHeight = 50f;
            largeText.HorizontalAlignment = HorizontalAlignment.Centered;
            largeText.VerticalAlignment = VerticalAlignment.Middle;
            largeText.Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.\nAliquam posuere sapien nibh, vel sodales sapien accumsan id.\nEtiam eleifend suscipit mauris quis dignissim.\nCras in convallis velit, non dignissim nisi.\nNullam sed tortor rhoncus, tempus magna nec, semper mauris.\nProin consequat urna in tristique sollicitudin.\nNunc tempus nibh sed felis dapibus, ut imperdiet turpis gravida.\nPhasellus quis eros eget ipsum ultricies tempus in porttitor eros.\nÄäkköskääpiö sanoo öitä, ja örisee äänekkäästi.";

            fpsText = new GuiText();
            fpsText.Font = font;
            fpsText.FontSize = 10f;
            fpsText.Weight = 1f;
            fpsText.Smooth = 1.5f;
            fpsText.LineHeight = 1.0f;
            fpsText.Color = new Vector4(1, 0, 0, 1);
            fpsText.Text = "FPS: Calculating..";

            Debug.WriteLine("OnLoad() ends");
        }

        Vector2 mousepos;

        protected override void OnMouseMove(OpenTK.Input.MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);

            //float smooth = (float)e.X / (float)Width / 2;
            //float weight = (float)e.Y / (float)Height * 2;

            mousepos = new Vector2(e.X, e.Y);
        }

        bool bold = false;

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            
            if(e.KeyChar == 'b')
            {
                bold = !bold;

                if (bold)
                    largeText.Weight = 2.0f;
                else
                    largeText.Weight = 1.0f;
            }

        }

        double totalTime = 0;
        double frametime = 0;

        Task guiUpdate = null;

        int update = 0;

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            Time.Update(e.Time);

            GuiRoot.Root.UpdateAsync(e.Time);

            base.OnUpdateFrame(e);

            camera.Perspective(Width, Height, 45, 1.0f, 1000.0f);
            

            totalTime+=e.Time * 0.25;
            //camera.Position = new Vector3((float)(Math.Sin(totalTime * 2) * 2.5f), (float)(Math.Cos(totalTime * 2.25) * 2.5f), (float)(Math.Sin(totalTime) * 2.5f));

            /*if (UserInput.Mouse.IsButtonDown(OpenTK.Input.MouseButton.Left))
                camera.Position = new Vector3(2, 2, 2);*/

            camera.Update((float)e.Time);

            update++;

            //largeText.FontSize = 100f * (float)((Math.Sin(totalTime) + 1.15) / 2.0);

            //largeText.Weight = (2f - (largeText.FontSize / 100f));
        }
        
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            frame++;

            frametime += e.Time;

            if(frametime > 1.0)
            {
                frametime = 0;
                fpsText.Text = string.Format("FPS: {0}, Update: {1}", frame, update);
                frame = 0;
                update = 0;
            }

            text.Text = string.Format("Calculated weight: {0}", largeText.CalculatedWeight);

            GuiRoot.Root.WaitUpdateFinish();

            // It is important that this is right before main render thread starts working on current context.
            Time.Render(e.Time);

            // Wait for context
            while (!GLThreadHelper.MakeGLContextCurrent()) { }

            GL.Viewport(0, 0, Width, Height);

            GL.ClearColor(0.25f, 0.25f, 0.25f, 1);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            GL.Enable(EnableCap.DepthTest);
            //GL.Disable(EnableCap.Blend);

            var ray = camera.GetRayFromWindowPoint(UserInput.Mouse.Position);

            //var apina = ray.Position + ray.Direction * 5;
            var apina = Vector3.Zero;

            boldText.Text = string.Format("Ray p:{0} n:{1}\nObject position: {2}\nCaptured text:{3}", ray.Position, ray.Direction, apina, UserInput.Keyboard.CapturedText);

            Matrix4 model = Matrix4.CreateRotationX((float)Math.Sin(totalTime * 1.3f)) * Matrix4.CreateRotationY((float)Math.Cos(totalTime * 1.45f)) * Matrix4.CreateTranslation(apina) * camera.VPMatrix;

            modelShader.Enable();
            modelShader.SendUniform("MVP", ref model);
            modelShader.SendUniform("EyeDirection", ref camera.Direction);

            GL.ActiveTexture(TextureUnit.Texture0);
            texture.Bind();

            mesh.Draw();

            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            Matrix4 projection = Matrix4.CreateOrthographicOffCenter(0, Width, Height, 0, -1, 1);
            Matrix4 view = Matrix4.CreateTranslation(10, 30, 0);

            Matrix4 mvp = view * projection;

            var textureSize = new Vector2(text.Font.Texture.Width, text.Font.Texture.Height);

            textShader.Enable();
            textShader.SendUniform("MVP", ref mvp);
            textShader.SendUniform("textureSize", ref textureSize);
            GL.ActiveTexture(TextureUnit.Texture0);

            text.Draw(textShader);

            view = Matrix4.CreateTranslation(10, 60, 0);
            mvp = view * projection;

            textShader.SendUniform("MVP", ref mvp);

            boldText.Draw(textShader);

            view = Matrix4.CreateTranslation(Width / 2.0f, Height / 2.0f, 0);
            mvp = view * projection;

            textShader.SendUniform("MVP", ref mvp);

            //largeText.Draw(textShader);

            view = Matrix4.CreateTranslation(0, 10, 0);
            mvp = view * projection;

            textShader.SendUniform("MVP", ref mvp);

            fpsText.Draw(textShader);

            base.OnRenderFrame(e);

            //GL.Flush();
            SwapBuffers();

            GLThreadHelper.Unlock();

            UserInput.FrameEnd();
        }

        int frame;
    }
}

