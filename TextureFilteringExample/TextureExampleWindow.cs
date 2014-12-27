using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hatzap.Rendering;
using Hatzap.Shaders;
using Hatzap.Textures;
using Hatzap.Utilities;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace TextureExample
{
    class TextureExampleWindow : OpenTK.GameWindow
    {
        public TextureExampleWindow()
            : base(1280, 720, new GraphicsMode(new ColorFormat(32), 24, 8, 32, 0, 2, false), "Hatzap Texture Examples", GameWindowFlags.Default,
                DisplayDevice.GetDisplay(DisplayIndex.Default), 3, 3, GraphicsContextFlags.Default)
        { }

        VertexBatch batch;
        ShaderProgram shader;
        Texture texture;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Set GL state
            GLState.AlphaBleding = true;

            // Load shaders
            var collection = XML.Read.FromFile<ShaderCollection>("../../Assets/Shaders/collection.xml");
            ShaderManager.LoadCollection(collection);
            shader = ShaderManager.Get("texturedquad");

            // Create screen-filling quad
            batch = new VertexBatch();
            batch.StartBatch(PrimitiveType.TriangleStrip);
            batch.Add(new Vector3(-1, -1, 0)); // Bottom left
            batch.Add(new Vector3(1, -1, 0));  // Bottom right
            batch.Add(new Vector3(-1, 1, 0));  // Top left
            batch.Add(new Vector3(1, 1, 0));   // Top Right
            batch.EndBatch();

            // Load texture
            TextureMeta textureMeta = new TextureMeta()
            {
                Name = "Texture",
                FileName = "../../Assets/Textures/3D_pattern_textures_25/pattern_124/diffuse.png",
                PixelInternalFormat = PixelInternalFormat.Rgba,
                PixelFormat = PixelFormat.Bgra,
                PixelType = PixelType.UnsignedByte,
                Quality = new TextureQuality()
                {
                    Filtering = TextureFiltering.Nearest,
                    Anisotrophy = 0,
                    Mipmaps = false,
                }
            };

            texture = new Texture();
            texture.Load(textureMeta);
            texture.UpdateQuality();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            // Clear the screen
            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Bind the texture to use
            texture.Bind();

            // Enable the shader
            shader.Enable();

            // Render the batch
            batch.Render();

            // Clean up frame
            shader.Disable();
            texture.UnBind();

            // Display rendered frame
            SwapBuffers();
        }
    }
}
