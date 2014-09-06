using Hatzap.Shaders;
using Hatzap.Models;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Hatzap;
using System.Diagnostics;

namespace HatzapTestApplication
{
    class HatzapGameWindow : GameWindow
    {
        Camera camera;
        ShaderProgram skyShader;

        Vector2 viewPort;

        Mesh mesh;

        public HatzapGameWindow() : base(1280,720, GraphicsMode.Default, "Hatzap Test Application", GameWindowFlags.Default, 
            DisplayDevice.GetDisplay(DisplayIndex.Default), 3, 3, GraphicsContextFlags.Default)
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            Debug.WriteLine("OnLoad()");

            base.OnLoad(e);

            viewPort = new Vector2(Width, Height);

            camera = new Camera();
            camera.SetAsCurrent();

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

            skyShader = new ShaderProgram("Sky");
            skyShader.AttachShader(skyVertex);
            skyShader.AttachShader(skyFragment);
            skyShader.Link();
            skyShader.Enable();

            Vector3[] Data = new Vector3[] {
                new Vector3(-1.0f, -1.0f, 0.0f),
                new Vector3(1.0f, -1.0f, 0.0f),
                new Vector3(0.0f, 1.0f, 0.0f),
            };

            int[] Indices = new int[] {
                0,1,2
            };

            mesh = new Mesh();
            mesh.Vertices = Data;
            mesh.Indices = Indices;

            /*
            // Generate GL names
            GL.GenVertexArrays(1, out Vao);
            GL.GenBuffers(1, out Vbo);
            GL.GenBuffers(1, out Ebo);

            // bind the vao
            GL.BindVertexArray(Vao);

            // Bind Vertex buffer to vao
            GL.BindBuffer(BufferTarget.ArrayBuffer, Vbo);

            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(Data.Length * Vector4.SizeInBytes), Data, BufferUsageHint.StaticDraw);

            // Set the vbo settings in the vao
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, BlittableValueType.StrideOf(Data), 0);

            // Bind index buffer to vao
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, Ebo);

            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(Indices.Length * sizeof(uint)), Indices, BufferUsageHint.StaticDraw);

            // Unbind the vao
            GL.BindVertexArray(0);*/

            Debug.WriteLine("OnLoad() ends");
        }

        protected override void OnMouseMove(OpenTK.Input.MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);
        }
        
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            SwapBuffers();

            GL.ClearColor(0.5f, 0.5f, 0.5f, 1);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //GL.DepthMask(false);

            GL.Disable(EnableCap.CullFace);

            skyShader.SendUniform("InvProjection", ref camera.InvProjection);

            mesh.Draw();

            base.OnRenderFrame(e);

            GL.Flush();
            //SwapBuffers();
        }
    }
}

