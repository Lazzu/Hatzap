using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hatzap;
using Hatzap.Assets;
using Hatzap.Shaders;
using Hatzap.Sprites;
using Hatzap.Utilities;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace SplattyBird
{
    class SplattyBird : GameWindow
    {
        public SplattyBird()
            : base(1280, 720, new GraphicsMode(new ColorFormat(32), 24, 8, 16, 0, 2, false), "Hatzap Sprites Example", GameWindowFlags.Default,
                DisplayDevice.GetDisplay(DisplayIndex.Default), 3, 3, GraphicsContextFlags.Default)
        { }

        SpriteAtlasManager sam;
        SpriteBatch spriteBatch;
        SpriteAtlas spriteAtlas;
        Sprite spaceShip;
        Camera orthoCamera;

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);

            // Clean up any loaded assets
            AssetManagerBase.CleanupManagers();
        }

        protected override void OnLoad(EventArgs e)
        {
            // Initialize GL State
            GPUCapabilities.Initialize();
            GL.ClearColor(0.25f, 0.25f, 0.25f, 1.0f);

            // Set up PackageManager
            PackageManager.BasePath = "Assets/";

            // Initialize camera. Rest is in OnResize()
            orthoCamera = new Camera(this);
            orthoCamera.SetAsCurrent();

            // Load shaders
            ShaderManager.LoadCollection("Shaders/collection.xml");

            // Initialize batch and atlas manager
            spriteBatch = new SpriteBatch();
            sam = new SpriteAtlasManager();

            // Temporary example until editor gets sprite support
            // Uncomment to regenerate sprite file
            GenerateSpriteFile();

            // Get the sprite atlas
            spriteAtlas = sam.Get("Sprites/SplattySprites.spr", true);

            // Get sprite by ID (slow operation, cache result on load)
            spaceShip = spriteAtlas["PlayerBlue1"];
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            orthoCamera.Projection = Matrix4.CreateOrthographicOffCenter(-Width / 2.0f, Width / 2.0f, -Height / 2.0f, Height / 2.0f, -100, 100);
        }

        double totalTime;

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            totalTime += e.Time;

            // Clear screen
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Begin a sprite batch, only sprites from this atlas can be rendered
            spriteBatch.BeginBatch(spriteAtlas);

            // Calculate sprite drawing parameters
            // First sprite
            var position = new Vector2((float)Math.Sin(totalTime + Math.PI * 2 / 3) * 100f, (float)Math.Cos(totalTime + Math.PI * 2 / 3) * 100f);
            var scale = new Vector2(0.5f, 0.5f);
            var rotation = (float)(totalTime * Math.PI / 10f); // over 10 seconds
            var color = new Vector4(1f, 0.5f, 0.5f, 1f);
            // Second sprite
            var position2 = new Vector2((float)Math.Sin(totalTime) * 100f, (float)Math.Cos(totalTime) * 100f);
            var rotation2 = (float)(totalTime * Math.PI / -5f); // over 5 seconds (faster) opposite direction
            var color2 = new Vector4(0.5f, 0.5f, 1f, 1f);
            // Third sprite
            var position3 = new Vector2((float)Math.Sin(totalTime + Math.PI * 4 / 3) * 100f, (float)Math.Cos(totalTime + Math.PI * 4 / 3) * 100f);
            var rotation3 = (float)(totalTime * Math.PI / 3f); // over 3 seconds (fastest)
            var color3 = new Vector4(0.5f, 1f, 0.5f, 1f);

            // Draw sprites
            spriteBatch.Draw(spaceShip, position, scale, rotation, color);
            spriteBatch.Draw(spaceShip, position2, scale, rotation2, color2);
            spriteBatch.Draw(spaceShip, position3, scale, rotation3, color3);

            // End batch. The end result will be shown on active framebuffer at this point.
            spriteBatch.EndBatch();

            // Swap GL buffers
            SwapBuffers();
        }

        // This is a temporary solution until editor has support for sprite editing.
        private void GenerateSpriteFile()
        {
            // One texel unit
            float unit = 1f / 1024f;

            // Texture coordinates on the atlas
            float punkLeft = 0;
            float punkTop = 0;
            float punkRight = 128 * unit;
            float punkBottom = 128 * unit;
            float punkLeft2 = punkRight;
            float punkRight2 = punkRight + 128 * unit;

            var vertices = new Vector3[] {          // A sprite can have more vertices,
                            new Vector3(-0.5f, -0.5f, 0),   // useful for fill-rate optimization.
                            new Vector3(0.5f, -0.5f, 0),    // vertex position count and uv count
                            new Vector3(-0.5f, 0.5f, 0),    // must be equal. 4 vert -> 4 uv
                            new Vector3(0.5f, 0.5f, 0),
                        };

            // All sprites are children of the atlas they belong to
            spriteAtlas = new SpriteAtlas()
            {
                Sprites = new List<Sprite>()
                {
                    new Sprite() {
                        Name = "PunkBird.0", // Sprite ID in this atlas
                        Vertices = vertices,
                        TextureCoordinates = new Vector2[] {
                            new Vector2(punkLeft, punkTop),
                            new Vector2(punkRight, punkTop),
                            new Vector2(punkLeft, punkBottom),
                            new Vector2(punkRight, punkBottom),
                        },
                        Indices = new uint[] {
                            0,1,2,2,1,3
                        },
                        Size = new Vector2(99f, 75f)
                    },
                    new Sprite() {
                        Name = "PunkBird.1", // Sprite ID in this atlas
                        Vertices = vertices,
                        TextureCoordinates = new Vector2[] {
                            new Vector2(punkLeft2, punkTop),
                            new Vector2(punkRight2, punkTop),
                            new Vector2(punkLeft2, punkBottom),
                            new Vector2(punkRight2, punkBottom),
                        },
                        Indices = new uint[] {
                            0,1,2,2,1,3
                        },
                        Size = new Vector2(99f, 75f)
                    }
                },
                // A texture path that will get loaded automatically when the atlas gets loaded
                TextureName = "Textures/SplattyAtlas.tex",

                // Our texels does not have premultiplied alpha
                PremultipliedAlpha = false,
            };

            // Insert in temporary location and save to filesystem
            sam.Insert("tmp", spriteAtlas);
            sam.Save("tmp", PackageManager.BasePath + "Sprites/SplattySprites.spr");
            sam.Remove("tmp");
        }
    }
}
