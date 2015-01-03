﻿using System;
using System.Collections.Generic;
using Hatzap;
using Hatzap.Assets;
using Hatzap.Models;
using Hatzap.Rendering;
using Hatzap.Shaders;
using Hatzap.Textures;
using Hatzap.Utilities;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace ModelExample
{
    class ModelExampleWindow : GameWindow
    {
        public ModelExampleWindow()
            : base(1280, 720, new GraphicsMode(new ColorFormat(32), 24, 8, 16, 0, 2, false), "Hatzap Model Example", GameWindowFlags.Default,
                DisplayDevice.GetDisplay(DisplayIndex.Default), 3, 3, GraphicsContextFlags.Default)
        { }

        Model model;
        ShaderProgram shader;
        Texture texture;
        Mesh mesh;
        Camera camera;
        RenderQueue renderQueue;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            PackageManager.BasePath = "../../Assets/";

            GLState.DepthTest = true;
            GLState.AlphaBleding = false;
            GLState.CullFace = true;

            GL.ClearColor(0.25f, 0.25f, 0.25f, 1.0f);

            camera = new Camera(this);
            camera.Perspective(Width, Height, (float)Math.PI / 4, 1f, 1000);
            camera.Position = new Vector3(0, 20, 200);
            camera.Target = new Vector3(0, 0, -1);
            camera.SetAsCurrent();

            // Load shaders
            ShaderManager.LoadCollection("Shaders/collection.xml");
            shader = ShaderManager.Get("simplemodel");

            // Load texture
            TextureMeta textureMeta = new TextureMeta()
            {
                FileName = "../../Assets/Textures/3D_pattern_textures_25/pattern_124/diffuse.png",
                PixelInternalFormat = PixelInternalFormat.Rgba,
                PixelFormat = PixelFormat.Bgra,
                PixelType = PixelType.UnsignedByte,
                Quality = new TextureQuality()
                {
                    Filtering = TextureFiltering.Trilinear,
                    Anisotrophy = 16,
                    Mipmaps = true,
                }
            };

            texture = new Texture();
            texture.Load(textureMeta);

            MeshManager meshManager = new MeshManager();

            mesh = meshManager.Get("Meshes/suzanne.mesh", true);

            model = new Model()
            {
                Shader = shader,
                Texture = texture,
                Mesh = mesh,
                Material = new Material()
            };
            
            renderQueue = new RenderQueue();
            renderQueue.AllowInstancing = true;
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            camera.Perspective(Width, Height, (float)Math.PI / 4, 1f, 1000);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            camera.Update((float)e.Time);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            model.Transform.Rotation *= new Quaternion(0, (float)e.Time * 0.2f, 0);

            renderQueue.Insert(model);
            renderQueue.Render();

            SwapBuffers();
        }
    }
}
