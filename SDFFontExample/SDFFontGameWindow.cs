using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hatzap.Assets;
using Hatzap.Gui;
using Hatzap.Gui.Fonts;
using Hatzap.Shaders;
using Hatzap.Textures;
using Hatzap.Utilities;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace SDFFontExample
{
    class SDFFontGameWindow : GameWindow
    {
        public SDFFontGameWindow()
            : base(1280, 720, new GraphicsMode(new ColorFormat(32), 24, 8, 16, 0, 2, false), "Hatzap Hello Triangle", GameWindowFlags.Default,
                DisplayDevice.GetDisplay(DisplayIndex.Default), 3, 3, GraphicsContextFlags.Default)
        { }

        ShaderProgram shader;
        Font font;
        GuiText text;
        TextureManager textures;

        protected override void OnLoad(EventArgs e)
        {
            PackageManager.BasePath = "../../Assets/";

            ShaderManager.LoadCollection("Shaders/collection.xml");

            shader = ShaderManager.Get("Text");

            //FontManager.LoadCollection("Fonts/fontCollection.fnt");
            //font = FontManager.Get("Arial");

            textures = new TextureManager();

            font = new Font();
            font.LoadBMFont("Fonts/OpenSans-Regular.ttf_sdf.txt");
            font.Texture = textures.Get("Textures/OpenSans-Regular.ttf_sdf.tex", true);

            text = new GuiText();
            text.Font = font;
            
            text.Text = "This is rendered with SDF technology! (now you should say ooooooooh!)";

            text.HorizontalAlignment = HorizontalAlignment.Centered;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            textures.ReleaseAll();
        }

        double totalTime;

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            // Clear the screen
            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            totalTime += e.Time;

            // This stuff should be abstracted away
            shader.Enable();
            GLState.DepthTest = false;
            GLState.AlphaBleding = true;
            GLState.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            Matrix4 projection = Matrix4.CreateOrthographicOffCenter(-Width/2.0f, Width/2.0f, Height, -Height/2.0f, -1, 1);
            Matrix4 view = Matrix4.Identity;
            var textureSize = new Vector2(text.Font.Texture.Width, text.Font.Texture.Height);
            view = Matrix4.CreateTranslation(0, 10, 0);
            var mvp = view * projection;
            shader.SendUniform("MVP", ref mvp);
            shader.SendUniform("textureSize", ref textureSize);
            GL.ActiveTexture(TextureUnit.Texture0);

            // This is horrible, but here for demonstration. When text color changes, the whole 
            // mesh gets regenerated which is slow if done too much or with too long texts
            float r = (float)((Math.Sin(totalTime * 3.2398457) + 1) / 2);
            float g = (float)((Math.Sin(totalTime * 2.5547642) + 1) / 2);
            float b = (float)((Math.Sin(totalTime * 4.2567432) + 1) / 2);
            text.Color = new Vector4(r, g, b, 1);

            // Set font size
            text.FontSize = (float)(Math.Sin(totalTime) * 20.0 + 10.0);

            // Draw the text
            text.Draw(shader);

            shader.Disable();

            // Display rendered frame
            SwapBuffers();
        }
    }
}
