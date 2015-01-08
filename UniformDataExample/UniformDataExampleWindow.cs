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

        Model model, model2, model3;
        Camera camera;
        RenderQueue renderQueue;
        TextureManager textures;
        MaterialManager materials;

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);

            AssetManagerBase.CleanupManagers();
        }

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

            // Load texture
            textures = new TextureManager();
            var texture = textures.Get("Textures/concreteslabs.tex", true);
            texture.Quality.Mipmaps = true;
            texture.Quality.TextureWrapMode = TextureWrapMode.Repeat;

            // Load up a mesh
            MeshManager meshManager = new MeshManager();
            var mesh = meshManager.Get("Meshes/dragon.mesh", true);

            // Create base material
            materials = new MaterialManager();
            materials.Insert("uniformexamplematerial.mat", new Material()
            {
                UniformData = new List<IUniformData>()
                {
                    new UniformDataFloat() {
                        Name = "time",
                        Data = 0.0f
                    },
                    new UniformDataVector2() {
                        Name = "toffset",
                        Data = Vector2.Zero
                    },
                    new UniformDataVector4() {
                        Name = "color",
                        Data = Vector4.One
                    }
                }
            });

            // Construct a model from shader, texture and mesh, with default material
            model = new Model()
            {
                Shader = ShaderManager.Get("uniformexamplemodel"),
                Texture = texture,
                Mesh = mesh,
                Material = materials.Get("uniformexamplematerial.mat")
            };

            model2 = new Model()
            {
                Shader = ShaderManager.Get("uniformexamplemodel"),
                Texture = texture,
                Mesh = mesh,
                Material = materials.Get("uniformexamplematerial.mat")
            };

            model3 = new Model()
            {
                Shader = ShaderManager.Get("uniformexamplemodel"),
                Texture = texture,
                Mesh = mesh,
                Material = materials.Get("uniformexamplematerial.mat")
            };

            model.Transform.Position += new Vector3(-1, 0.5f, 0);
            model2.Transform.Position += new Vector3(1, 0.5f, 0);
            model3.Transform.Position += new Vector3(0, -0.75f, 0);

            model.Transform.Rotation *= new Quaternion(0, (float)Math.PI, 0);

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

        double totalTime = 0;

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            totalTime += e.Time;

            // Update camera
            camera.Update((float)e.Time);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Modify uniforms
            var uniform = model.Material["time"] as UniformDataFloat;
            uniform.Data += (float)e.Time;

            var uniform2 = model2.Material["toffset"] as UniformDataVector2;
            uniform2.Data = new Vector2((float)Math.Sin(totalTime), (float)Math.Cos(totalTime));

            var uniform3 = model3.Material["color"] as UniformDataVector4;
            uniform3.Data = new Vector4(
                (float)(Math.Sin(totalTime) + 1.0) * 0.5f, 
                (float)(Math.Cos(totalTime) + 1.0) * 0.5f, 
                (float)(Math.Tan(totalTime) + 1.0) * 0.5f, 
                1.0f);


            // Render model
            renderQueue.Insert(model);
            renderQueue.Insert(model2);
            renderQueue.Insert(model3);
            renderQueue.Render();

            SwapBuffers();
        }
    }
}
