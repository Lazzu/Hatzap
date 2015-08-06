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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultitexturingExample
{
    class MultitexturingExampleWindow : GameWindow
    {
        public MultitexturingExampleWindow()
            : base(1280, 720, new GraphicsMode(new ColorFormat(32), 24, 8, 16, 0, 2, false), "Hatzap Multitexturing Example", GameWindowFlags.Default,
                DisplayDevice.GetDisplay(DisplayIndex.Default), 3, 3, GraphicsContextFlags.Default)
        { }

        Model groundplane, model;
        Camera camera;
        RenderQueue renderQueue;
        TextureManager textures;

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
            GLState.CullFace = true;
            GLState.BlendFunc(BlendingFactorSrc.SrcColor, BlendingFactorDest.OneMinusSrcAlpha);
            GL.ClearColor(0.25f, 0.25f, 0.25f, 1.0f);

            // Set up camera
            camera = new Camera(this);
            camera.Perspective(Width, Height, (float)Math.PI / 4, 1f, 1000);
            camera.Position = new Vector3(0, 20, 200);
            camera.Target = new Vector3(0, 0, 0);
            camera.SetAsCurrent();

            // Set up PackageManager
            PackageManager.BasePath = "../../Assets/";

            // Load shaders
            ShaderManager.LoadCollection("Shaders/collection.xml");

            // Load texture
            textures = new TextureManager();
            var texture = textures.Get("Textures/lucymetal.tex", true);
            texture.Quality.Mipmaps = true;

            var groundtexture = textures.Get("Textures/concreteslabs.tex", true);
            groundtexture.Quality.Mipmaps = true;
            groundtexture.Quality.TextureWrapMode = TextureWrapMode.Repeat;

            var grasstexture = textures.Get("Textures/grass.tex", true);
            grasstexture.Quality.Mipmaps = true;
            grasstexture.Quality.TextureWrapMode = TextureWrapMode.Repeat;

            // Load up a mesh
            MeshManager meshManager = new MeshManager();
            var mesh = meshManager.Get("Meshes/lucy.mesh", true);
            var groundmesh = meshManager.Get("Meshes/plane.mesh", true);

            var modelMaterial = new Material()
                {
                    ShaderProgram = ShaderManager.Get("transparentmodel"),
                    Transparent = false,
                    UniformData = new List<IUniformData>()
                    {
                        new UniformDataVector4() {
                            Name = "color",
                            Data = new Vector4(1,1,1,0.75f),
                        },
                        new UniformDataFloat() {
                            Name = "specularIntensity",
                            Data = 0.5f,
                        }
                    }
                };

            modelMaterial.Textures.Add("textureSampler", texture);

            // Construct a model from shader, texture and mesh, with default material
            model = new Model()
            {
                Mesh = mesh,
                Material = modelMaterial
            };

            var groundMaterial = new Material()
                {
                    ShaderProgram = ShaderManager.Get("multitexturedGround"),
                };
            groundMaterial.Textures.Add("stoneTexture", groundtexture);
            groundMaterial.Textures.Add("grassTexture", grasstexture);
            //groundMaterial.Textures.Add("splatTexture", groundtexture);

            groundplane = new Model()
            {
                Mesh = groundmesh,
                Material = groundMaterial
            };

            model.Transform.Position += new Vector3(0, 0, 0);
            groundplane.Transform.Position += new Vector3(0, -1, 0);

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
            camera.Position = new Vector3((float)Math.Sin(totalTime * 0.1f) * 200, 100, (float)Math.Cos(totalTime * 0.1f) * 200);
            camera.Update((float)e.Time);

            // Clear render buffer
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Rotate model around Y axis
            model.Transform.Rotation *= new Quaternion(0, (float)e.Time * 0.2f, 0);

            // Render model
            renderQueue.Insert(model);
            renderQueue.Insert(groundplane);
            renderQueue.Render();

            SwapBuffers();
        }
    }
}
