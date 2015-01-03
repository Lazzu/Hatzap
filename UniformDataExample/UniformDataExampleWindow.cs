using System;
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

namespace UniformDataExample
{
    public class UniformDataExampleWindow : GameWindow
    {
        public UniformDataExampleWindow()
            : base(1280, 720, new GraphicsMode(new ColorFormat(32), 24, 8, 16, 0, 2, false), "Hatzap Uniform Example", GameWindowFlags.Default,
                DisplayDevice.GetDisplay(DisplayIndex.Default), 3, 3, GraphicsContextFlags.Default)
        { }

        Model model;
        Camera camera;
        RenderQueue renderQueue;

        protected override void OnLoad(EventArgs e)
        {
            // Initialize GL settings
            GPUCapabilities.Initialize();
            GLState.DepthTest = true;
            GLState.AlphaBleding = false;
            GLState.CullFace = true;
            GL.ClearColor(0.25f, 0.25f, 0.25f, 1.0f);

            // Set up camera
            camera = new Camera(this);
            camera.Perspective(Width, Height, (float)Math.PI / 4, 1f, 1000);
            camera.Position = new Vector3(0, 20, 100);
            camera.Target = new Vector3(0, 0, -1);
            camera.SetAsCurrent();

            // Set up PackageManager
            PackageManager.BasePath = "../../Assets/";

            // Load shaders
            ShaderManager.LoadCollection("Shaders/collection.xml");
            var shader = ShaderManager.Get("uniformexamplemodel");

            // Load texture
            TextureMeta textureMeta = new TextureMeta()
            {
                FileName = "../../Assets/Textures/3D_pattern_textures_25/pattern_128/diffuse.png",
                PixelInternalFormat = PixelInternalFormat.CompressedRgbaS3tcDxt1Ext,
                PixelFormat = PixelFormat.Bgra,
                PixelType = PixelType.UnsignedByte,
                Quality = new TextureQuality()
                {
                    Filtering = TextureFiltering.Trilinear,
                    Anisotrophy = 16,
                    Mipmaps = true,
                }
            };
            var texture = new Texture();
            texture.Load(textureMeta);

            // Load up a mesh
            MeshManager meshManager = new MeshManager();
            var mesh = meshManager.Get("Meshes/dragon.mesh", true);

            // Construct a model from shader, texture and mesh, with default material
            model = new Model()
            {
                Shader = shader,
                Texture = texture,
                Mesh = mesh,
                Material = new Material()
                {
                    UniformData = new List<IUniformData>()
                    {
                        new UniformDataFloat() {
                            Name = "time",
                            Data = 0.0f
                        }
                    }
                }
            };

            // set up rendering queue
            renderQueue = new RenderQueue();
            renderQueue.AllowInstancing = false;
        }

        protected override void OnResize(EventArgs e)
        {
            // Update viewport and camera settings
            GL.Viewport(0, 0, Width, Height);
            camera.Perspective(Width, Height, (float)Math.PI / 2.0f, 1f, 1000);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            // Update camera
            camera.Update((float)e.Time);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Modify uniform
            var uniform = model.Material["time"] as UniformDataFloat;
            uniform.Data += (float)e.Time;

            // Rotate the model around Y axis
            model.Transform.Rotation *= new Quaternion(0, (float)e.Time * 0.2f, 0);

            // Render model
            renderQueue.Insert(model);
            renderQueue.Render();

            SwapBuffers();
        }
    }
}
