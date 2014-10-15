﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Assimp;
using Assimp.Configs;
using Hatzap;
using Hatzap.Gui;
using Hatzap.Gui.Anchors;
using Hatzap.Gui.Fonts;
using Hatzap.Gui.Widgets;
using Hatzap.Input;
using Hatzap.Models;
using Hatzap.Rendering;
using Hatzap.Scenes;
using Hatzap.Shaders;
using Hatzap.Textures;
using Hatzap.Utilities;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

using Quaternion = OpenTK.Quaternion;

namespace HatzapTestApplication
{
    class HatzapGameWindow : GameWindow
    {
        Hatzap.Camera camera;
        ShaderProgram skyShader;
        ShaderProgram modelShader;
        ShaderProgram textShader;

        Vector2 viewPort;

        Hatzap.Models.Mesh mesh;

        Texture texture, normalTexture;

        TextureArray shipTexture;

        Hatzap.Gui.Fonts.Font font;
        GuiText text;
        GuiText boldText;
        GuiText largeText;
        GuiText fpsText;

        Model[] spaceShips;

        Scene scene = new Scene();

        public HatzapGameWindow() : base(1280,720, new GraphicsMode(new ColorFormat(32), 24, 8, 32, 0, 2, false), "Hatzap Test Application", GameWindowFlags.Default, 
            DisplayDevice.GetDisplay(DisplayIndex.Default), 3, 3, GraphicsContextFlags.Default)
        {
            //WindowState = OpenTK.WindowState.Maximized;
        }

        protected override void OnLoad(EventArgs e)
        {
            Debug.WriteLine("OnLoad()");

            GPUCapabilities.Initialize();

            Debug.WriteLine("GPUCapabilities.Version=" + GPUCapabilities.Version);
            Debug.WriteLine("GPUCapabilities.GLSL=" + GPUCapabilities.GLSL);
            Debug.WriteLine("GPUCapabilities.Instancing=" + GPUCapabilities.Instancing);
            Debug.WriteLine("GPUCapabilities.MaxVaryingFloats=" + GPUCapabilities.MaxVaryingFloats);
            Debug.WriteLine("GPUCapabilities.MaxVaryingVectors=" + GPUCapabilities.MaxVaryingVectors);

            GLState.DepthTest = true;
            GLState.AlphaBleding = true;
            GLState.CullFace = true;

            SceneManager.Initialize(500, 10, 10, Vector3.Zero);
            SceneManager.CullByObject = false;

            viewPort = new Vector2(Width, Height);

            camera = new Hatzap.Camera(this);
            
            camera.SetAsCurrent();

            camera.Position = new Vector3(1, 1, 1);
            camera.Target = new Vector3(0, 0, 0);

            camera.Update(0);
            camera.DirectionLock = true;

            camera.Rotate(new Vector2(-(float)Math.PI / 2.5f, 0));

            // Now camera.Position changes whenever Target changes, and the camera angle stays locked.
            // Camera's distance from it's target can be controlled from camera.Distance property now.

            FontCollection fonts = new FontCollection();
            
            fonts.Fonts.Add(new FontInfo()
            {
                FontFamily = "OpenSans-Regular",
                FontDataFile = "Assets/Fonts/OpenSans-Regular.ttf_sdf.txt",
                FontTextureFile = "Assets/Fonts/OpenSans-Regular.ttf_sdf.png"
            });

            XML.Write.ToFile(fonts, "Assets/Fonts/collection.xml");

            FontManager.LoadCollection(fonts);

            ShaderCollection collection = new ShaderCollection();

            collection.ShaderPrograms.Add(new ShaderProgramInfo()
            {
                Shaders = new List<ShaderInfo>(new[]{ new ShaderInfo() {
                    Path = "Assets/Shaders/Model.vert",
                    Type = ShaderType.VertexShader
                },new ShaderInfo() {
                    Path = "Assets/Shaders/ModelColored.frag",
                    Type = ShaderType.FragmentShader
                }}),
                Name = "Model"
            });

            collection.ShaderPrograms.Add(new ShaderProgramInfo()
            {
                Shaders = new List<ShaderInfo>(new[]{ new ShaderInfo() {
                    Path = "Assets/Shaders/Text.vert",
                    Type = ShaderType.VertexShader
                },new ShaderInfo() {
                    Path = "Assets/Shaders/Text.frag",
                    Type = ShaderType.FragmentShader
                }}),
                Name = "Text"
            });

            collection.ShaderPrograms.Add(new ShaderProgramInfo()
            {
                Shaders = new List<ShaderInfo>(new[]{ new ShaderInfo() {
                    Path = "Assets/Shaders/Gui.vert",
                    Type = ShaderType.VertexShader
                },new ShaderInfo() {
                    Path = "Assets/Shaders/Gui.frag",
                    Type = ShaderType.FragmentShader
                }}),
                Name = "Gui"
            });

            collection.ShaderPrograms.Add(new ShaderProgramInfo()
            {
                Shaders = new List<ShaderInfo>(new[]{ new ShaderInfo() {
                    Path = "Assets/Shaders/GuiImage.vert",
                    Type = ShaderType.VertexShader
                },new ShaderInfo() {
                    Path = "Assets/Shaders/GuiImage.frag",
                    Type = ShaderType.FragmentShader
                }}),
                Name = "Gui.Image"
            });

            collection.ShaderPrograms.Add(new ShaderProgramInfo()
            {
                Shaders = new List<ShaderInfo>(new[]{ new ShaderInfo() {
                    Path = "Assets/Shaders/SimpleModel.vert",
                    Type = ShaderType.VertexShader
                },new ShaderInfo() {
                    Path = "Assets/Shaders/SimpleModel.frag",
                    Type = ShaderType.FragmentShader
                }}),
                Name = "SimpleModel"
            });

            //XML.Write.ToFile(collection, "Assets/Shaders/collection.xml");

            ShaderManager.LoadCollection(collection);

            Time.Initialize();
            UserInput.Initialize(this, typeof(AccurateMouse), typeof(Hatzap.Input.Keyboard));
            UserInput.Mouse.ClickInterval = 0.5f;
            GuiRoot.Initialize(this);

            GuiRoot.Root.Texture = new TextureArray();

            GuiRoot.Root.Texture.Load(new[] { new Bitmap("Assets/Textures/gui.png") }, PixelInternalFormat.Rgba, PixelFormat.Bgra, PixelType.UnsignedByte);

            GuiRoot.Root.Texture.TextureSettings(TextureMinFilter.Nearest, TextureMagFilter.Nearest, 0);

            ElementCollection guiElements = new ElementCollection()
            {
                Elements = new List<WidgetInfo> { 
                    new WidgetInfo(){
                        WidgetType = typeof(Button).ToString(),
                        Slices = new List<GuiTextureRegion> {
                            new GuiTextureRegion() { // Top left
                                Offset = new Vector2(19,0),
                                Size = new Vector2(5,5),
                                Page = 0
                            }, new GuiTextureRegion() { // Top center
                                Offset = new Vector2(24,0),
                                Size = new Vector2(1,5),
                                Page = 0
                            }, new GuiTextureRegion() { // Top Right
                                Offset = new Vector2(25,0),
                                Size = new Vector2(5,5),
                                Page = 0
                            }, new GuiTextureRegion() { // Middle left
                                Offset = new Vector2(19,5),
                                Size = new Vector2(5,1),
                                Page = 0
                            }, new GuiTextureRegion() { // Middle center
                                Offset = new Vector2(24,5),
                                Size = new Vector2(1,1),
                                Page = 0
                            }, new GuiTextureRegion() { // Middle right
                                Offset = new Vector2(25,5),
                                Size = new Vector2(5,1),
                                Page = 0
                            }, new GuiTextureRegion() { // Bottom left
                                Offset = new Vector2(19,6),
                                Size = new Vector2(5,9),
                                Page = 0
                            }, new GuiTextureRegion() { // Bottom center
                                Offset = new Vector2(24,6),
                                Size = new Vector2(1,9),
                                Page = 0
                            }, new GuiTextureRegion() { // Bottom right
                                Offset = new Vector2(25,6),
                                Size = new Vector2(5,9),
                                Page = 0
                            },
                        },
                    },
                    new WidgetInfo(){
                        WidgetType = typeof(Window).ToString(),
                        Slices = new List<GuiTextureRegion> {
                            new GuiTextureRegion() { // Top left
                                Offset = new Vector2(0,0),
                                Size = new Vector2(9,9),
                                Page = 0
                            }, new GuiTextureRegion() { // Top center
                                Offset = new Vector2(9,0),
                                Size = new Vector2(1,9),
                                Page = 0
                            }, new GuiTextureRegion() { // Top Right
                                Offset = new Vector2(10,0),
                                Size = new Vector2(9,9),
                                Page = 0
                            }, new GuiTextureRegion() { // Middle left
                                Offset = new Vector2(0,9),
                                Size = new Vector2(9,1),
                                Page = 0
                            }, new GuiTextureRegion() { // Middle center
                                Offset = new Vector2(9,9),
                                Size = new Vector2(1,1),
                                Page = 0
                            }, new GuiTextureRegion() { // Middle right
                                Offset = new Vector2(10,9),
                                Size = new Vector2(9,1),
                                Page = 0
                            }, new GuiTextureRegion() { // Bottom left
                                Offset = new Vector2(0,10),
                                Size = new Vector2(9,2),
                                Page = 0
                            }, new GuiTextureRegion() { // Bottom center
                                Offset = new Vector2(9,10),
                                Size = new Vector2(1,1),
                                Page = 0
                            }, new GuiTextureRegion() { // Bottom right
                                Offset = new Vector2(10,10),
                                Size = new Vector2(9,1),
                                Page = 0
                            }, new GuiTextureRegion() { // Bottom left
                                Offset = new Vector2(0,11),
                                Size = new Vector2(9,14),
                                Page = 0
                            }, new GuiTextureRegion() { // Bottom center
                                Offset = new Vector2(9,11),
                                Size = new Vector2(1,14),
                                Page = 0
                            }, new GuiTextureRegion() { // Bottom right
                                Offset = new Vector2(10,11),
                                Size = new Vector2(9,14),
                                Page = 0
                            },
                        },
                    },
                }
            };

            XML.Write.ToFile(guiElements, "Assets/Gui/elements.xml");

            #region GridContainer test
            GridContainer grid = new GridContainer();
            grid.Columns = 3;
            grid.Rows = 2;
            {
                Button btn = new Button();
                Button btn2 = new Button();
                Button btn3 = new Button();
                Button btn4 = new Button();
                Button btn5 = new Button();
                Button btn6 = new Button();
                
                grid.AddChildWidget(btn);
                grid.AddChildWidget(btn2);
                grid.AddChildWidget(btn3);
                grid.AddChildWidget(btn4);
                grid.AddChildWidget(btn5);
                grid.AddChildWidget(btn6);

                btn.Color = new Vector4(1, 0.5f, 0.5f, 1);
                btn.Text = "Click\nEvent";
                btn.GuiText.LineHeight = 60;
                btn.OnClick += (m) =>
                {
                    //btn.Text = "Clicked " + m.ToString();
                    Random r = new Random();
                    btn.Color = new Vector4((float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble(), 1);
                };
                btn.Anchor = new Anchor();
                btn.Anchor.Directions[AnchorDirection.Left] = AnchorType.Snap;
                btn.Anchor.Directions[AnchorDirection.Right] = AnchorType.Snap;
                btn.Anchor.Directions[AnchorDirection.Top] = AnchorType.Snap;
                btn.Anchor.Directions[AnchorDirection.Bottom] = AnchorType.Snap;
                btn.Position = new Vector2(100, 100);
                btn.Size = new Vector2(150, 50);
                btn.TextureRegion = guiElements.Elements[0].Slices.ToArray();


                btn2.Text = "Down\nEvent";
                btn2.GuiText.LineHeight = 60;
                btn2.OnDown += (m) =>
                {
                    //btn2.Text = "Clicked " + m.ToString();
                    Random r = new Random();
                    btn2.Color = new Vector4((float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble(), 1);
                };
                btn2.Anchor = new Anchor();
                btn2.Anchor.Directions[AnchorDirection.Left] = AnchorType.Snap;
                btn2.Anchor.Directions[AnchorDirection.Right] = AnchorType.Snap;
                btn2.Anchor.Directions[AnchorDirection.Top] = AnchorType.Snap;
                btn2.Anchor.Directions[AnchorDirection.Bottom] = AnchorType.Snap;
                btn2.Position = new Vector2(300, 100);
                btn2.Size = new Vector2(150, 50);
                btn2.TextureRegion = guiElements.Elements[0].Slices.ToArray();


                btn3.Text = "Up\nEvent";
                btn3.GuiText.LineHeight = 60;
                btn3.OnUp += (m) =>
                {
                    Random r = new Random();
                    btn3.Color = new Vector4((float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble(), 1);
                    //btn3.Z = btn4.Z + 1;
                };
                btn3.Anchor = new Anchor();
                btn3.Anchor.Directions[AnchorDirection.Left] = AnchorType.Snap;
                btn3.Anchor.Directions[AnchorDirection.Right] = AnchorType.Snap;
                btn3.Anchor.Directions[AnchorDirection.Top] = AnchorType.Snap;
                btn3.Anchor.Directions[AnchorDirection.Bottom] = AnchorType.Snap;
                btn3.Position = new Vector2(100, 200);
                btn3.Size = new Vector2(150, 50);
                btn3.TextureRegion = guiElements.Elements[0].Slices.ToArray();


                btn4.Text = "Leave\nEvent";
                btn4.GuiText.LineHeight = 60;
                btn4.OnClick += (m) =>
                {
                    //btn4.Text = "Clicked " + m.ToString();
                    Random r = new Random();
                    btn4.Color = new Vector4((float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble(), 1);
                    //btn4.Z = btn3.Z + 1;
                };
                btn4.OnLeave += () =>
                {
                    Random r = new Random();
                    btn4.Color = new Vector4((float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble(), 1);
                };
                btn4.Anchor = new Anchor();
                btn4.Anchor.Directions[AnchorDirection.Left] = AnchorType.Snap;
                btn4.Anchor.Directions[AnchorDirection.Right] = AnchorType.Snap;
                btn4.Anchor.Directions[AnchorDirection.Top] = AnchorType.Snap;
                btn4.Anchor.Directions[AnchorDirection.Bottom] = AnchorType.Snap;
                btn4.Position = new Vector2(150, 200);
                btn4.Size = new Vector2(150, 50);
                btn4.TextureRegion = guiElements.Elements[0].Slices.ToArray();

                btn5.Text = "Enter\nEvent";
                btn5.GuiText.LineHeight = 60;
                btn5.OnClick += (m) =>
                {
                    Random r = new Random();
                    btn5.Color = new Vector4((float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble(), 1);
                    //btn3.Z = btn4.Z + 1;
                };
                btn5.OnEnter += () => 
                {
                    Random r = new Random();
                    btn5.Color = new Vector4((float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble(), 1);
                };
                btn5.Anchor = new Anchor();
                btn5.Anchor.Directions[AnchorDirection.Left] = AnchorType.Snap;
                btn5.Anchor.Directions[AnchorDirection.Right] = AnchorType.Snap;
                btn5.Anchor.Directions[AnchorDirection.Top] = AnchorType.Snap;
                btn5.Anchor.Directions[AnchorDirection.Bottom] = AnchorType.Snap;
                btn5.Position = new Vector2(100, 200);
                btn5.Size = new Vector2(150, 50);
                btn5.TextureRegion = guiElements.Elements[0].Slices.ToArray();


                btn6.Text = "Hover\nEvent";
                btn6.GuiText.LineHeight = 60;
                btn6.OnClick += (m) =>
                {
                    //btn4.Text = "Clicked " + m.ToString();
                    Random r = new Random();
                    btn6.Color = new Vector4((float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble(), 1);
                    //btn4.Z = btn3.Z + 1;
                };
                btn6.OnHover += () =>
                {
                    Random r = new Random();
                    btn6.Color = new Vector4((float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble(), 1);
                };
                btn6.Anchor = new Anchor();
                btn6.Anchor.Directions[AnchorDirection.Left] = AnchorType.Snap;
                btn6.Anchor.Directions[AnchorDirection.Right] = AnchorType.Snap;
                btn6.Anchor.Directions[AnchorDirection.Top] = AnchorType.Snap;
                btn6.Anchor.Directions[AnchorDirection.Bottom] = AnchorType.Snap;
                btn6.Position = new Vector2(150, 200);
                btn6.Size = new Vector2(150, 50);
                btn6.TextureRegion = guiElements.Elements[0].Slices.ToArray();
            }

            grid.Position = new Vector2(150, 100);
            grid.Size = new Vector2(400, 200);

            grid.LeftAnchorOffset = 5;
            grid.TopAnchorOffset = 5;
            #endregion

            #region StackContainer test
            StackContainer stack = new StackContainer();
            {
                Button btn = new Button();
                Button btn2 = new Button();
                Button btn3 = new Button();
                Button btn4 = new Button();

                stack.AddChildWidget(btn);
                stack.AddChildWidget(btn2);
                stack.AddChildWidget(btn3);
                stack.AddChildWidget(btn4);

                btn.Color = new Vector4(1.5f, 1.5f, 1.5f, 1);
                btn.Text = "Button 1";
                btn.TextColor = new Vector4(0, 0, 0, 1);
                btn.OnClick += (m) =>
                {
                    //btn.Text = "Clicked " + m.ToString();
                };
                btn.Anchor = new Anchor();
                btn.Anchor.Directions[AnchorDirection.Left] = AnchorType.Snap;
                btn.Anchor.Directions[AnchorDirection.Right] = AnchorType.Snap;
                btn.Anchor.Directions[AnchorDirection.Top] = AnchorType.Snap;
                btn.Position = new Vector2(100, 100);
                btn.Size = new Vector2(150, 50);
                btn.TextureRegion = guiElements.Elements[0].Slices.ToArray();



                btn2.Color = new Vector4(0.2f, 0.2f, 0.2f, 1);
                btn2.Text = "Button 2";
                btn2.OnClick += (m) =>
                {
                    btn2.Text = "Clicked " + m.ToString();
                };
                btn2.Anchor = new Anchor();
                btn2.Anchor.Directions[AnchorDirection.Left] = AnchorType.Snap;
                btn2.Anchor.Directions[AnchorDirection.Right] = AnchorType.Snap;
                btn2.Anchor.Directions[AnchorDirection.Top] = AnchorType.Snap;
                btn2.Position = new Vector2(300, 100);
                btn2.Size = new Vector2(150, 50);
                btn2.TextureRegion = guiElements.Elements[0].Slices.ToArray();


                btn3.Text = "Button 3";
                btn3.OnClick += (m) =>
                {
                    Random r = new Random();
                    btn3.Color = new Vector4((float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble(), 1);
                    //btn3.Z = btn4.Z + 1;
                };
                btn3.Anchor = new Anchor();
                btn3.Anchor.Directions[AnchorDirection.Left] = AnchorType.Snap;
                btn3.Anchor.Directions[AnchorDirection.Right] = AnchorType.Snap;
                btn3.Anchor.Directions[AnchorDirection.Top] = AnchorType.Snap;
                btn3.Position = new Vector2(100, 200);
                btn3.Size = new Vector2(150, 50);
                btn3.TextureRegion = guiElements.Elements[0].Slices.ToArray();

                btn4.Color = new Vector4(1, 1, 1, 0.5f);
                btn4.Text = "Button 4";
                btn4.OnClick += (m) =>
                {
                    //btn4.Text = "Clicked " + m.ToString();
                    //btn4.Z = btn3.Z + 1;
                };
                btn4.Anchor = new Anchor();
                btn4.Anchor.Directions[AnchorDirection.Left] = AnchorType.Snap;
                btn4.Anchor.Directions[AnchorDirection.Right] = AnchorType.Snap;
                btn4.Anchor.Directions[AnchorDirection.Top] = AnchorType.Snap;
                btn4.Position = new Vector2(150, 200);
                btn4.Size = new Vector2(150, 50);
                btn4.TextureRegion = guiElements.GetInfo(btn4).Slices.ToArray();
            }

            stack.Position = new Vector2(600, 100);
            stack.Size = new Vector2(400, 200);

            stack.LeftAnchorOffset = 5;
            stack.TopAnchorOffset = 5;
            #endregion

            var image = new Hatzap.Gui.Widgets.Image();
            var lblText = new Label();

            Window window = new Window();
            window.TextureRegion = guiElements.GetInfo(window).Slices.ToArray();
            window.Position = new Vector2(1200, 600);
            window.Size = new Vector2(300, 200);
            window.TitleHeight = 20;
            window.TitleColor = new Vector4(79f / 255f / 0.5f, 193f / 255f / 0.5f, 233f / 255f / 0.5f, 1f);
            window.Color = new Vector4(1f / (210f / 255f), 1f / (210f / 255f), 1f / (210f / 255f), 1f);

            //GuiRoot.Root.AddWidget(grid);
            //GuiRoot.Root.AddWidget(stack);
            //GuiRoot.Root.AddWidget(window);
            //GuiRoot.Root.AddWidget(image);
            //GuiRoot.Root.AddWidget(lblText);
            
            image.Texture = new Texture();
            image.Texture.Load(new Bitmap("Assets/Textures/Default.png"), PixelFormat.Bgra, PixelType.UnsignedByte);
            image.Texture.Bind();
            image.Texture.TextureSettings(TextureMinFilter.Linear, TextureMagFilter.Linear, 32);
            image.Position = new Vector2(100, 500);
            image.Size = new Vector2(100, 100);
            
            
            lblText.Text = "This is a GUI Label";
            lblText.Position = new Vector2(800, 50);
            lblText.GuiText.HorizontalAlignment = HorizontalAlignment.Left;

            
            

            UserInput.Keyboard.CaptureText = true;

            modelShader = ShaderManager.Get("Model");
            textShader = ShaderManager.Get("Text");

            //Create a new importer
            AssimpContext importer = new AssimpContext();

            //This is how we add a configuration (each config is its own class)
            //NormalSmoothingAngleConfig config = new NormalSmoothingAngleConfig(66.0f);
            //importer.SetConfig(config);

            var flags = PostProcessPreset.TargetRealTimeMaximumQuality | PostProcessSteps.Triangulate | PostProcessSteps.SortByPrimitiveType | PostProcessSteps.FlipUVs;

            //Import the model. All configs are set. The model
            //is imported, loaded into managed memory. Then the unmanaged memory is released, and everything is reset.
            Scene model = importer.ImportFile("Assets/Models/Sample_Ship.fbx", flags);
            
            mesh = new Hatzap.Models.Mesh();

            Assimp.Mesh aMesh = null;

            foreach (var item in model.Meshes)
            {
                if (item.PrimitiveType != Assimp.PrimitiveType.Triangle)
                    continue;

                aMesh = item;
                break;
            }

            if (aMesh != null)
            {
                mesh.AssimpMesh = aMesh;
            }
            else
            {
                Debug.WriteLine("ERROR: No triangle meshes found in imported model.");
            }

            //End of example
            importer.Dispose();

            Debug.WriteLine("Compressed texture support: " + GPUCapabilities.TextureCompression);

            var bitmaps = new[] { 
                new Bitmap("Assets/Textures/sh3.jpg"), 
                new Bitmap("Assets/Textures/sh3_n.png"),
                new Bitmap("Assets/Textures/sh3_s.jpg")
            };

            shipTexture = new TextureArray();
            shipTexture.PixelInternalFormat = PixelInternalFormat.CompressedRgbS3tcDxt1Ext;
            shipTexture.Load(bitmaps, PixelInternalFormat.Rgba, PixelFormat.Bgra, PixelType.UnsignedByte);
            shipTexture.TextureSettings(TextureMinFilter.Linear, TextureMagFilter.Linear, 32);

            font = FontManager.Get("OpenSans-Regular");

            text = new GuiText();
            text.Font = font;
            text.FontSize = 10f;
            text.Weight = 1f;
            text.Smooth = 0.5f;
            text.Border = 0;
            text.Text = "\"The quick brown fox jumps over the lazy dog\" is an English-language pangram—a phrase that contains all of the letters of the alphabet.";

            fpsText = new GuiText();
            fpsText.Font = font;
            fpsText.FontSize = 8f;
            fpsText.Weight = 1.2f;
            fpsText.Smooth = 2.5f;
            fpsText.LineHeight = 50.0f;
            fpsText.Color = new Vector4(1, 1, 1, 1);
            fpsText.Text = "FPS: Calculating..";

            int n = 10;
            int s = 5;

            Hatzap.Models.Material spaceShipMaterial = new Hatzap.Models.Material();

            spaceShipMaterial.UniformData = new List<IUniformData> { 
                        new UniformDataVector4()
                        {
                            Name = "Color",
                            Data = new Vector4(1.0f, 1.0f, 1.0f, 1.0f)
                        }
                    };

            for (int x = -n; x <= n; x++)
            for (int y = -n; y <= n; y++)
            for (int z = -n; z <= n; z++)
            {
                var spaceShip = new Model();
                spaceShip.Texture = shipTexture;
                spaceShip.Shader = ShaderManager.Get("SimpleModel");
                spaceShip.Mesh = mesh;
                spaceShip.Transform.Static = true;
                spaceShip.Transform.Position = new Vector3(x * s, y * s, z * s);
                spaceShip.Transform.Rotation = Quaternion.FromEulerAngles(x * 360.0f / n / (float)Math.PI, y * 360.0f / n / (float)Math.PI, z * 360.0f / n / (float)Math.PI);

                spaceShip.Material = spaceShipMaterial;

                SceneManager.Insert(spaceShip);
            }

            Debug.WriteLine("OnLoad() ends");
            
            base.OnLoad(e);
        }

        Vector2 mousepos;

        protected override void OnMouseMove(OpenTK.Input.MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);

            //float smooth = (float)e.X / (float)Width / 2;
            //float weight = (float)e.Y / (float)Height * 2;

            mousepos = new Vector2(e.X, e.Y);

        }

        bool bold = false;

        Vector3 moveVector = Vector3.Zero;

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            
            if(e.KeyChar == 'b')
            {
                bold = !bold;

                if (bold)
                    largeText.Weight = 2.0f;
                else
                    largeText.Weight = 1.0f;
            }

        }

        double totalTime = 0;
        double frametime = 0;
        double measureTime = 0;
        
        int update = 0;
        
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            Time.StartTimer("UpdateFrame()", "Loop");

            Time.Update(e.Time);

            // Must be the first thing called after Time.Update();
            UserInput.Update();

            // Must be called as early as possible, but after input update.
            GuiRoot.Root.UpdateAsync(e.Time);

            
            base.OnUpdateFrame(e);

            SceneManager.Update();
            SceneManager.QueueForRendering(Hatzap.Camera.Current);

            totalTime += e.Time * 0.25;

            camera.Perspective(Width, Height, 60, 1.0f, 100.0f);
            camera.Update((float)e.Time);

            update++;
            
            //camera.Rotate(new Vector2(0, (mousepos.X / (float)Width - 0.5f) * 0.5f * (float)e.Time));
            camera.Rotate(new Vector2(0,(float)e.Time * 0.1f));
            camera.Distance = (float)(Math.Sin(totalTime * 0.25f) + 1.2f) * 10;

            Time.StopTimer("UpdateFrame()");
        }
        
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            Time.StartTimer("RenderFrame();", "Loop");

            Time.StartTimer("SwapBuffers();", "Render");
            SwapBuffers();
            Time.StopTimer("SwapBuffers();");

            frame++;

            frametime += e.Time;
            measureTime += e.Time;

            if(frametime > 1.0)
            {
                frametime = 0;

                fps = frame;

                Title = fps.ToString();

                frame = 0;
                update = 0;
            }

            if(measureTime > 0.1)
            {
                GenerateFpsText();
                measureTime = 0;
            }
            

            //text.Text = string.Format("Calculated weight: {0}", largeText.CalculatedWeight);

            // It is important that this is right before main render thread starts working on current context.
            Time.Render(e.Time);

            Time.StartTimer("Overhead", "Overhead");
            GLState.DepthTest = true;
            GL.Viewport(0, 0, Width, Height);
            GL.ClearColor(0.25f, 0.25f, 0.25f, 1);
            Time.StopTimer("Overhead");

            Time.StartTimer("glClear()", "Overhead");
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            Time.StopTimer("glClear()");
            

            base.OnRenderFrame(e);

            RenderQueue.Render();

            // Wait for gui update in case it was done in a background thread
            GuiRoot.Root.WaitUpdateFinish();

            Time.StartTimer("Overhead", "Overhead");
            GLState.DepthTest = false;
            GLState.AlphaBleding = true;
            GLState.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            Time.StopTimer("Overhead");

            GuiRoot.Root.Render();

            DrawFpsText();

            Time.StartTimer("glFlush()", "Render");
            GL.Flush();
            Time.StopTimer("glFlush()");

            UserInput.FrameEnd();
            Time.StopTimer("RenderFrame();");
        }

        [Conditional("DEBUG")]
        private void DrawFpsText()
        {
            Time.StartTimer("DrawMeasurementsText()", "Render");
            Matrix4 projection = Matrix4.CreateOrthographicOffCenter(0, Width, Height, 0, -1, 1);
            Matrix4 view = Matrix4.CreateTranslation(10, 30, 0);
            var textureSize = new Vector2(text.Font.Texture.Width, text.Font.Texture.Height);
            textShader.Enable();
            view = Matrix4.CreateTranslation(0, 10, 0);
            var mvp = view * projection;
            textShader.SendUniform("MVP", ref mvp);
            textShader.SendUniform("textureSize", ref textureSize);
            GL.ActiveTexture(TextureUnit.Texture0);
            fpsText.Draw(textShader);
            Time.StopTimer("DrawMeasurementsText()");
        }

        StringBuilder sb = new StringBuilder();

        [Conditional("DEBUG")]
        private void GenerateFpsText()
        {
            Time.StartTimer("GenerateFpsText()", "Update");

            sb.Clear();

            sb.Append("FPS: ").Append(fps).Append("\n");
            sb.Append("RenderQueue count: ").Append(RenderQueue.Count).Append("\n");
            sb.Append("RenderQueue triangles: ").Append(RenderQueue.TrianglesDrawn).Append("\n");
            sb.Append("Scenemanager object count: ").Append(SceneManager.ObjectCount).Append("\n");
            sb.Append("\nTimers:\n");


            foreach (var item in Time.History)
            {
                double average = 0, total = 0;
                int n = 0;
                int count = 0;
                foreach (var measures in item.Value)
                {
                    total = 0;
                    count = 0;
                    foreach (var time in measures)
                    {
                        average += time;
                        total += time;
                        count++;
                    }
                    n++;
                }
                //if (count > 0)
                {
                    average = Math.Round(average / n, 4);
                    total = Math.Round(total / count, 4);
                    sb.Append(item.Key).Append(": ").Append(total).Append(" ms, count: ").Append(count).Append(", total average: ").Append(average).Append(" ms").Append("\n");
                }
            }

            fpsText.Text = sb.ToString();

            Time.StopTimer("GenerateFpsText()");
        }

        int frame, fps;
    }
}


