using Hatzap.Shaders;
using Hatzap.Models;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using Hatzap;
using System.Diagnostics;
using Assimp;
using Assimp.Configs;

namespace HatzapTestApplication
{
    class HatzapGameWindow : GameWindow
    {
        Hatzap.Camera camera;
        ShaderProgram skyShader;
        ShaderProgram modelShader;

        Vector2 viewPort;

        Hatzap.Models.Mesh mesh;

        public HatzapGameWindow() : base(1280,720, GraphicsMode.Default, "Hatzap Test Application", GameWindowFlags.Default, 
            DisplayDevice.GetDisplay(DisplayIndex.Default), 3, 3, GraphicsContextFlags.Default)
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            Debug.WriteLine("OnLoad()");

            base.OnLoad(e);

            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);

            viewPort = new Vector2(Width, Height);

            camera = new Hatzap.Camera();
            
            camera.SetAsCurrent();

            camera.Position = new Vector3(0, 0, 2);
            camera.Target = new Vector3(0, 0, 0);

            /*Shader skyVertex = new Shader(ShaderType.VertexShader);
            Shader skyFragment = new Shader(ShaderType.FragmentShader);

            using (StreamReader r = new StreamReader("Assets/Shaders/Sky.vert"))
            {
                skyVertex.ShaderSource(r.ReadToEnd());
            }

            using (StreamReader r = new StreamReader("Assets/Shaders/Sky.frag"))
            {
                skyFragment.ShaderSource(r.ReadToEnd());
            }

            skyShader = new ShaderProgram("Sky");
            skyShader.AttachShader(skyVertex);
            skyShader.AttachShader(skyFragment);
            skyShader.Link();
            skyShader.Enable();*/

            Shader modelVertexShader = new Shader(ShaderType.VertexShader);
            Shader modelFragmentShader = new Shader(ShaderType.FragmentShader);

            using (StreamReader r = new StreamReader("Assets/Shaders/Model.vert"))
            {
                modelVertexShader.ShaderSource(r.ReadToEnd());
            }

            using (StreamReader r = new StreamReader("Assets/Shaders/Model.frag"))
            {
                modelFragmentShader.ShaderSource(r.ReadToEnd());
            }

            modelShader = new ShaderProgram("Model");
            modelShader.AttachShader(modelVertexShader);
            modelShader.AttachShader(modelFragmentShader);
            modelShader.Link();
            modelShader.Enable();

            Vector3[] Data = new Vector3[] {
                new Vector3(-1.0f, -1.0f, 0.0f),
                new Vector3(1.0f, -1.0f, 0.0f),
                new Vector3(0.0f, 1.0f, 0.0f),
            };

            int[] Indices = new int[] {
                0,1,2
            };

            //Create a new importer
            AssimpContext importer = new AssimpContext();

            //This is how we add a configuration (each config is its own class)
            NormalSmoothingAngleConfig config = new NormalSmoothingAngleConfig(66.0f);
            importer.SetConfig(config);

            //This is how we add a logging callback 
            LogStream logstream = new LogStream(delegate(String msg, String userData)
            {
                Debug.WriteLine(msg);
            });
            logstream.Attach();

            //Import the model. All configs are set. The model
            //is imported, loaded into managed memory. Then the unmanaged memory is released, and everything is reset.
            Scene model = importer.ImportFile("Assets/Models/suzanne.fbx", PostProcessPreset.TargetRealTimeMaximumQuality | PostProcessSteps.CalculateTangentSpace);
            
            var verts = model.Meshes[0].Vertices;
            var normals = model.Meshes[0].Normals;
            var tangents = model.Meshes[0].Tangents;
            var binormals = model.Meshes[0].BiTangents;
            var tcoord = model.Meshes[0].TextureCoordinateChannels[0];
            var colors = model.Meshes[0].VertexColorChannels[0];

            mesh = new Hatzap.Models.Mesh();

            Vector3[] v = new Vector3[verts.Count];
            Vector3[] n = new Vector3[verts.Count];
            Vector3[] t = new Vector3[verts.Count];
            Vector3[] bn = new Vector3[verts.Count];
            Vector2[] uv = new Vector2[verts.Count];
            Vector4[] c = new Vector4[verts.Count];

            for (int i = 0; i < verts.Count; i++ )
            {
                v[i] = new Vector3(verts[i].X, verts[i].Y, verts[i].Z);
                n[i] = new Vector3(normals[i].X, normals[i].Y, normals[i].Z);
                t[i] = new Vector3(tangents[i].X, tangents[i].Y, tangents[i].Z);
                bn[i] = new Vector3(binormals[i].X, binormals[i].Y, binormals[i].Z);
                uv[i] = new Vector2(tcoord[i].X, tcoord[i].Y);
                //uv[i] = new Vector2(0,0);
                c[i] = new Vector4(colors[i].R, colors[i].G, colors[i].B, colors[i].A);
                //Debug.WriteLine(uv[i]);
            }

            mesh.Vertices = v;
            mesh.Normals = n;
            mesh.Tangents = t;
            mesh.Binormals = bn;
            mesh.UV = uv;
            mesh.Colors = c;
            mesh.Indices = model.Meshes[0].GetIndices();

            //End of example
            importer.Dispose();

            Debug.WriteLine("OnLoad() ends");
        }

        protected override void OnMouseMove(OpenTK.Input.MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);
        }

        double totalTime = 0;

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            camera.Perspective(Width, Height, 0.0001f, 1000.0f);
            

            totalTime+=e.Time * 0.25;
            camera.Position = new Vector3((float)(Math.Sin(totalTime * 2) * 2.5f), (float)(Math.Cos(totalTime * 2.25) * 2.5f), (float)(Math.Sin(totalTime) * 2.5f));

            camera.Update((float)e.Time);
        }
        
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            SwapBuffers();

            GL.Viewport(0, 0, Width, Height);

            //GL.ClearColor(0.5f, 0.5f, 0.5f, 1);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //GL.DepthMask(false);

            //GL.Disable(EnableCap.CullFace);

            Matrix4 mat = Matrix4.Identity;

            modelShader.Enable();
            modelShader.SendUniform("MVP", ref camera.VPMatrix);

            mesh.Draw();

            base.OnRenderFrame(e);

            GL.Flush();
            //SwapBuffers();
        }
    }
}

