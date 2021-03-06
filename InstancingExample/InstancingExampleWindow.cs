﻿using System;
using System.Collections.Generic;
using Hatzap;
using Hatzap.Assets;
using Hatzap.Models;
using Hatzap.Rendering;
using Hatzap.Scenes;
using Hatzap.Shaders;
using Hatzap.Textures;
using Hatzap.Utilities;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace InstancingExample
{
    public class InstancingExampleWindow : GameWindow
    {
        public InstancingExampleWindow()
            : base(1280, 720, new GraphicsMode(new ColorFormat(32), 24, 8, 16, 0, 2, false), "Hatzap Instancing Example", GameWindowFlags.Default,
                DisplayDevice.GetDisplay(DisplayIndex.Default), 3, 3, GraphicsContextFlags.Default)
        { }

        Model[] spaceShips;
        RenderQueue renderQueue;
        Camera camera;
        TextureManager textures;
        MeshManager meshes;

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            camera.Perspective(Width, Height, (float)Math.PI / 2.0f, 10, 1000);
        }

        protected override void OnLoad(EventArgs e)
        {
            PackageManager.BasePath = "../../Assets/";

            GPUCapabilities.Initialize();
            GLState.DepthTest = true;
            GLState.CullFace = true;
            GL.ClearColor(0.25f, 0.25f, 0.25f, 1.0f);

            ShaderManager.LoadCollection("Shaders/collection.xml");

            SceneManager.Initialize(500, 5, 20, Vector3.Zero);
            SceneManager.CullByObject = false;

            renderQueue = new RenderQueue();
            renderQueue.AllowInstancing = true;

            meshes = new MeshManager();

            camera = new Hatzap.Camera(this);
            camera.SetAsCurrent();
            camera.Position = new Vector3(0, 1, 1);
            camera.Target = new Vector3(-1, 0, 0);
            camera.Update(0);
            camera.Rotate(new Vector2(-(float)Math.PI / 2.5f, 0));

            var rand = new Hatzap.Utilities.Random();

            int n = 8;
            float sizeScale = 10.0f;

            // All objects will have the same material
            Material material = new Material();

            material.ShaderProgram = ShaderManager.Get("instancedmodel");

            // To disable instancing, use these:
            //material.ShaderProgram = ShaderManager.Get("simplemodel");
            //renderQueue.AllowInstancing = false;


            for (int x = -n; x <= n; x++)
            {
                for (int y = -n; y <= n; y++)
                {
                    for (int z = -n; z <= n; z++)
                    {
                        var instancedObject = new Model();

                        // Set shader, mesh and material (no texture)
                        instancedObject.Mesh = meshes.Get("Meshes/suzanne.mesh", true);
                        instancedObject.Material = material.Copy();

                        // Set transform
                        instancedObject.Transform.Position = new Vector3(
                            (x + (float)(rand.NextDouble() - 0.5)) * sizeScale, 
                            (y + (float)(rand.NextDouble() - 0.5)) * sizeScale, 
                            (z + (float)(rand.NextDouble() - 0.5)) * sizeScale);
                        instancedObject.Transform.Rotation = Quaternion.FromEulerAngles(
                            x * 360.0f / n / (float)Math.PI, 
                            y * 360.0f / n / (float)Math.PI, 
                            z * 360.0f / n / (float)Math.PI);

                        float color = 1f;
                        color = color - ((y + n) / (n * 2.0f));

                        instancedObject.Material.Add(new UniformDataVector4(){
                            Data = new Vector4(color, color, color, 1),
                            Name = "Color",
                        });

                        // Don't calculate matrices on every frame
                        instancedObject.Transform.Static = true;

                        // Insert model in the scene octree. (Because I'm lazy and didn't want 
                        // to write code to update all these meshes in the queue by hand. Also
                        // culling is done by scene automatically.)
                        SceneManager.Insert(instancedObject);
                    }
                }
            }
        }

        double totalTime;

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            totalTime += e.Time;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Set camera for frame
            camera.Perspective(Width, Height, 30, 10.0f, 200.0f);
            camera.Position = new Vector3((float)Math.Sin(totalTime * 0.1) * 40, 10, (float)Math.Cos(totalTime * 0.1) * 40);
            camera.Update((float)e.Time);

            // Update scene (i.e. calculate non-static objects' matrices every frame) and queue objects culled by camera
            SceneManager.Update();
            SceneManager.QueueForRendering(camera, renderQueue);

            // Render everything from the scene
            renderQueue.Render();

            SwapBuffers();
        }
    }
}
