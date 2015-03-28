﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace FramebufferExample
{
    class FramebufferExampleWindow : GameWindow
    {
        public FramebufferExampleWindow()
            : base(1280, 720, new GraphicsMode(new ColorFormat(32), 0, 0, 0, 0, 2, false), "Hatzap Framebuffer Example", GameWindowFlags.Default,
                DisplayDevice.GetDisplay(DisplayIndex.Default), 3, 3, GraphicsContextFlags.Default)
        { }

        VertexBatch batch;
        ShaderProgram fboShader;
        Framebuffer fbo;
        Model model;
        private Camera camera;
        private TextureManager textures;
        private RenderQueue renderQueue;

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

            // Initialize framebuffer
            fbo = new Framebuffer(Width, Height);

            // Load other stuff
            LoadMeshStuff();

            
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
            renderQueue = new RenderQueue();
            renderQueue.AllowInstancing = false;
        }

        protected override void OnResize(EventArgs e)
        {
            // Update viewport and camera settings
            GL.Viewport(0, 0, Width, Height);
            camera.Perspective(Width, Height, (float)Math.PI / 4, 1f, 1000);

            // Release old and generate new framebuffer
            fbo.Release();
            fbo = new Framebuffer(Width, Height);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            // Rotate model around Y axis
            model.Transform.Rotation *= new Quaternion(0, (float)e.Time * 0.2f, 0);

            // Update camera
            camera.Update((float)e.Time);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            // Clear the screen, just the color since we don't have depth in the OS framebuffer
            GL.Clear(ClearBufferMask.ColorBufferBit);

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

            // Fill screen with FBO using some shader.
            fbo.RenderOnScreen(fboShader);

            // Display rendered frame
            SwapBuffers();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);

            AssetManagerBase.CleanupManagers();
        }
    }
}