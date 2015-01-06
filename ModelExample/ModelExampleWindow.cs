using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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
        Camera camera;
        RenderQueue renderQueue;
        TextureManager textures = new TextureManager();

        protected override void OnLoad(EventArgs e)
        {
            // Initialize GL settings
            GPUCapabilities.Initialize();
            GLState.DepthTest = true;
            GLState.CullFace = true;
            GLState.BlendFunc(BlendingFactorSrc.DstAlpha, BlendingFactorDest.OneMinusDstAlpha);
            GL.ClearColor(0.25f, 0.25f, 0.25f, 1.0f);

            // Set up camera
            camera = new Camera(this);
            camera.Perspective(Width, Height, (float)Math.PI / 4, 1f, 1000);
            camera.Position = new Vector3(0, 20, 200);
            camera.Target = new Vector3(0, 0, -1);
            camera.SetAsCurrent(); // Use this camera for rendering

            // Set up PackageManager
            PackageManager.BasePath = "../../Assets/";

            // Load shaders
            ShaderManager.LoadCollection("Shaders/collection.xml");
            var shader = ShaderManager.Get("simplemodel");

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
                Shader = shader,
                Texture = texture,
                Mesh = mesh,
                Material = new Material()
            };
            
            // set up rendering queue
            renderQueue = new RenderQueue();
            renderQueue.AllowInstancing = false;
        }

        protected override void OnResize(EventArgs e)
        {
            // Update viewport and camera settings
            GL.Viewport(0, 0, Width, Height);
            camera.Perspective(Width, Height, (float)Math.PI / 4, 1f, 1000);
        }

        double totalTime = 0;

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            totalTime += e.Time;

            // Update camera
            camera.Update((float)e.Time);

            // Clear render buffer
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Rotate model around Y axis
            model.Transform.Rotation *= new Quaternion(0, (float)e.Time * 0.2f, 0);
            
            // Render model
            renderQueue.Insert(model);
            renderQueue.Render();

            SwapBuffers();
        }
    }
}
