using System;
using Hatzap.Assets;
using Hatzap.Rendering;
using Hatzap.Shaders;
using Hatzap.Utilities;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace HelloTriangle
{
    class HelloTriangleWindow : GameWindow
    {
        public HelloTriangleWindow()
            : base(1280, 720, new GraphicsMode(new ColorFormat(32), 24, 8, 16, 0, 2, false), "Hatzap Hello Triangle", GameWindowFlags.Default,
                DisplayDevice.GetDisplay(DisplayIndex.Default), 3, 3, GraphicsContextFlags.Default)
        { }

        VertexBatch batch;
        ShaderProgram shader;

        protected override void OnLoad(EventArgs e)
        {
            PackageManager.BasePath = "../../Assets/";

            // Load shaders
            ShaderManager.LoadCollection("Shaders/collection.xml");
            shader = ShaderManager.Get("hellotriangle");

            // Generate triangle
            batch = new VertexBatch();
            batch.StartBatch(PrimitiveType.Triangles);
            batch.Add(new Vector3(-1, -1, 0)); // Bottom left
            batch.Add(new Vector3(1, -1, 0));  // Bottom right
            batch.Add(new Vector3(0, 1, 0));  // Top
            batch.EndBatch();
        }
        
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            // Clear the screen
            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            // Enable the shader
            shader.Enable();

            // Render the batch
            batch.Render();

            // Clean up frame
            shader.Disable();

            // Display rendered frame
            SwapBuffers();
        }
    }
}
