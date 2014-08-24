using Hatzap.Shaders;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HatzapTestApplication
{
    class HatzapGameWindow : GameWindow
    {
        ShaderProgram skyShader;

        public HatzapGameWindow() : base(1280,720, GraphicsMode.Default, "Hatzap Test Application", GameWindowFlags.Default, 
            DisplayDevice.GetDisplay(DisplayIndex.Default), 3, 3, GraphicsContextFlags.Default)
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Shader skyVertex = new Shader(ShaderType.VertexShader);
            Shader skyFragment = new Shader(ShaderType.FragmentShader);

            using (StreamReader r = new StreamReader("Assets/Shaders/Sky.vert"))
            {
                skyVertex.ShaderSource(r.ReadToEnd());
            }

            using(StreamReader r = new StreamReader("Assets/Shaders/Sky.frag"))
            {
                skyFragment.ShaderSource(r.ReadToEnd());
            }

            skyShader = new ShaderProgram();
            skyShader.AttachShader(skyVertex);
            skyShader.AttachShader(skyFragment);
            skyShader.Link();


        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            SwapBuffers();
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            skyShader.Enable();
            GL.DepthMask(false);

            base.OnRenderFrame(e);

            GL.Flush();
        }
    }
}

