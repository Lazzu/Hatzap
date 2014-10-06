using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
using Hatzap.Shaders;
using Hatzap.Textures;
using Hatzap.Utilities;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

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

        Model spaceShip;

        public HatzapGameWindow() : base(1280,720, new GraphicsMode(new ColorFormat(32), 32, 32, 16, new ColorFormat(8, 8, 8, 8), 2, false), "Hatzap Test Application", GameWindowFlags.Default, 
            DisplayDevice.GetDisplay(DisplayIndex.Default), 3, 3, GraphicsContextFlags.ForwardCompatible)
        {
            //WindowState = OpenTK.WindowState.Maximized;
        }

        protected override void OnLoad(EventArgs e)
        {
            Debug.WriteLine("OnLoad()");

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            //GL.Enable(EnableCap.VertexProgramPointSize);
            GL.Enable(EnableCap.CullFace);
            //GL.Enable(EnableCap.ScissorTest);

            viewPort = new Vector2(Width, Height);

            camera = new Hatzap.Camera(this);
            
            camera.SetAsCurrent();

            camera.Position = new Vector3(10, 10, 10);
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

            //XML.Write.ToFile(collection, "Assets/Shaders/collection.xml");

            ShaderManager.LoadCollection(collection);

            Time.Initialize();
            UserInput.Initialize(this, typeof(AccurateMouse), typeof(Hatzap.Input.Keyboard));
            //GLThreadHelper.Initialize(this);
            GuiRoot.Initialize(this);

            GuiRoot.Root.Texture = new TextureArray();

            GuiRoot.Root.Texture.Load(new[] { new Bitmap("Assets/Textures/gui.png") }, PixelInternalFormat.Rgba, PixelFormat.Bgra, PixelType.UnsignedByte);

            GuiRoot.Root.Texture.TextureSettings(TextureMinFilter.Linear, TextureMagFilter.Linear, 0);

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
                btn.Text = "Button";
                btn.OnClick += (m) =>
                {
                    //btn.Text = "Clicked " + m.ToString();
                };
                btn.Anchor = new Anchor();
                btn.Anchor.Directions[AnchorDirection.Left] = AnchorType.Snap;
                btn.Anchor.Directions[AnchorDirection.Right] = AnchorType.Snap;
                btn.Anchor.Directions[AnchorDirection.Top] = AnchorType.Snap;
                btn.Anchor.Directions[AnchorDirection.Bottom] = AnchorType.Snap;
                btn.Position = new Vector2(100, 100);
                btn.Size = new Vector2(150, 50);
                btn.TextureRegion = guiElements.Elements[0].Slices.ToArray();




                btn2.Text = "Button";
                btn2.OnClick += (m) =>
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


                btn3.Text = "Button";
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
                btn3.Anchor.Directions[AnchorDirection.Bottom] = AnchorType.Snap;
                btn3.Position = new Vector2(100, 200);
                btn3.Size = new Vector2(150, 50);
                btn3.TextureRegion = guiElements.Elements[0].Slices.ToArray();


                btn4.Text = "Button";
                btn4.OnClick += (m) =>
                {
                    //btn4.Text = "Clicked " + m.ToString();
                    Random r = new Random();
                    btn4.Color = new Vector4((float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble(), 1);
                    //btn4.Z = btn3.Z + 1;
                };
                btn4.Anchor = new Anchor();
                btn4.Anchor.Directions[AnchorDirection.Left] = AnchorType.Snap;
                btn4.Anchor.Directions[AnchorDirection.Right] = AnchorType.Snap;
                btn4.Anchor.Directions[AnchorDirection.Top] = AnchorType.Snap;
                btn4.Anchor.Directions[AnchorDirection.Bottom] = AnchorType.Snap;
                btn4.Position = new Vector2(150, 200);
                btn4.Size = new Vector2(150, 50);
                btn4.TextureRegion = guiElements.Elements[0].Slices.ToArray();

                btn5.Text = "Button";
                btn5.OnClick += (m) =>
                {
                    Random r = new Random();
                    btn5.Color = new Vector4((float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble(), 1);
                    //btn3.Z = btn4.Z + 1;
                };
                btn5.Anchor = new Anchor();
                btn5.Anchor.Directions[AnchorDirection.Left] = AnchorType.Snap;
                btn5.Anchor.Directions[AnchorDirection.Right] = AnchorType.Snap;
                btn5.Anchor.Directions[AnchorDirection.Top] = AnchorType.Snap;
                btn5.Anchor.Directions[AnchorDirection.Bottom] = AnchorType.Snap;
                btn5.Position = new Vector2(100, 200);
                btn5.Size = new Vector2(150, 50);
                btn5.TextureRegion = guiElements.Elements[0].Slices.ToArray();


                btn6.Text = "Button";
                btn6.OnClick += (m) =>
                {
                    //btn4.Text = "Clicked " + m.ToString();
                    Random r = new Random();
                    btn6.Color = new Vector4((float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble(), 1);
                    //btn4.Z = btn3.Z + 1;
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
                    //btn2.Text = "Clicked " + m.ToString();
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
                btn4.TextureRegion = guiElements.Elements[0].Slices.ToArray();
            }

            stack.Position = new Vector2(600, 100);
            stack.Size = new Vector2(400, 200);

            stack.LeftAnchorOffset = 5;
            stack.TopAnchorOffset = 5;
            #endregion

            var image = new Hatzap.Gui.Widgets.Image();
            var lblText = new Label();

            GuiRoot.Root.AddWidget(grid);
            GuiRoot.Root.AddWidget(stack);
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

            Debug.WriteLine("Compressed texture support: " + GPUCapabilities.IsExtensionAvailable("GL_EXT_texture_compression_s3tc"));

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

            boldText = new GuiText();
            boldText.Font = font;
            boldText.FontSize = 10f;
            boldText.Weight = 1.0f;
            boldText.Smooth = 0.5f;
            boldText.LineHeight = 50f;
            boldText.Text = "\"The quick brown fox jumps over the lazy dog\" is an English-language pangram—a phrase that contains all of the letters of the alphabet.";

            largeText = new GuiText();
            largeText.Font = font;
            largeText.FontSize = 30f;
            largeText.Weight = 1f;
            largeText.Smooth = 0.25f;
            largeText.Border = 0.5f;
            largeText.LineHeight = 50f;
            largeText.HorizontalAlignment = HorizontalAlignment.Centered;
            largeText.VerticalAlignment = VerticalAlignment.Middle;
            largeText.Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.\nAliquam posuere sapien nibh, vel sodales sapien accumsan id.\nEtiam eleifend suscipit mauris quis dignissim.\nCras in convallis velit, non dignissim nisi.\nNullam sed tortor rhoncus, tempus magna nec, semper mauris.\nProin consequat urna in tristique sollicitudin.\nNunc tempus nibh sed felis dapibus, ut imperdiet turpis gravida.\nPhasellus quis eros eget ipsum ultricies tempus in porttitor eros.\nÄäkköskääpiö sanoo öitä, ja örisee äänekkäästi.";

            fpsText = new GuiText();
            fpsText.Font = font;
            fpsText.FontSize = 8f;
            fpsText.Weight = 1.2f;
            fpsText.Smooth = 2.5f;
            fpsText.LineHeight = 50.0f;
            fpsText.Color = new Vector4(1, 1, 1, 1);
            fpsText.Text = "FPS: Calculating..";




            spaceShip = new Model();
            spaceShip.Texture = shipTexture;
            spaceShip.Shader = modelShader;
            spaceShip.Mesh = mesh;

            Debug.WriteLine("OnLoad() ends");

            RenderDataPool.MaxItems = 1000;

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
        
        int update = 0;

        double renderInsert = 0, renderQueue = 0, swapBufferTime = 0, guiwait = 0, garbage = 0, guiRender = 0;
        Stopwatch sw = new Stopwatch();
        Stopwatch swFrame = new Stopwatch();

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            UserInput.Update();

            sw.Reset();
            sw.Start();
            GC.Collect();
            sw.Stop();
            garbage = sw.Elapsed.TotalSeconds;

            Time.Update(e.Time);

            GuiRoot.Root.UpdateAsync(e.Time);

            base.OnUpdateFrame(e);

            totalTime += e.Time * 0.25;

            camera.Perspective(Width, Height, 45, 1.0f, 1000.0f);
            camera.Update((float)e.Time);

            update++;
            
            //camera.Rotate(new Vector2(0, (mousepos.X / (float)Width - 0.5f) * 0.5f * (float)e.Time));
            camera.Rotate(new Vector2(0,(float)e.Time * 0.1f));
            camera.Distance = (float)(Math.Sin(totalTime * 0.25f) + 1.2f) * 10;

            Random r = new Random();

            int n = (int)((Math.Sin(totalTime * 4) / 2 + 0.5) * 5);

            sw.Reset();
            sw.Start();

            for(int x = -n; x <= n; x++)
            {
                for(int y = -n; y <= n; y++)
                {
                    var data = RenderDataPool.GetInstance();

                    data.RenderObject = spaceShip;
                    var mM = Matrix4.CreateRotationX((float)Math.Sin(totalTime * x * 1.3f) * (float)Math.PI) * Matrix4.CreateRotationY((float)Math.Cos(totalTime * y * 1.45f) * (float)Math.PI) * Matrix4.CreateTranslation(new Vector3(x, 0, y));

                    Matrix4 mvp;
                    Matrix3 mN;
                    camera.GetModelViewProjection(ref mM, out mvp);
                    camera.GetNormalMatrix(ref mM, out mN);

                    data.UniformData = new List<IUniformData> { 
                        new UniformDataMatrix4()
                        {
                            Name = "MVP",
                            Data = mvp
                        },
                        new UniformDataMatrix3()
                        {
                            Name = "mN",
                            Data = mN
                        },
                        new UniformDataVector3() 
                        {
                            Name = "EyeDirection",
                            Data = camera.Direction
                        },
                        new UniformDataVector4()
                        {
                            Name = "Color",
                            Data = new Vector4(1.0f, 1.0f, 1.0f, 1.0f)
                        }
                    };

                    RenderQueue.Insert(data);
                }
            }

            sw.Stop();
            renderInsert = sw.Elapsed.TotalSeconds;
        }
        
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            sw.Reset();
            sw.Start();
            SwapBuffers();
            sw.Stop();
            swapBufferTime = sw.Elapsed.TotalSeconds;

            frame++;

            frametime += e.Time;

            swFrame.Stop();

            double frameTime = swFrame.Elapsed.TotalSeconds;

            swFrame.Reset();
            swFrame.Start();

            if(frametime > 1.0)
            {
                frametime = 0;
                double unknown = frameTime - swapBufferTime - renderQueue - renderInsert - guiwait;
                fpsText.Text = string.Format("FPS: {0}, Update: {1}\nFrame time: {6}\nRenderQueue count: {2}\nRenderInsert: {3}ms\nRenderQueue.Render: {4}ms\nSwapBuffers(): {5}ms\nUnknown: {7}ms\n" +
                    "Triangles Drawn: {8}\nObjectPool reserve: {9}\nObjectPool capacity: {10}\nGui Update: {11}ms\nGui Rebuild: {12}ms\nGui wait: {13}ms\nGui Render: {15}ms\nGC.Collect(): {14}ms", 
                    frame, update, RenderQueue.Count, Math.Round(renderInsert * 1000, 2), Math.Round(renderQueue * 1000, 2), Math.Round(swapBufferTime * 1000, 2), Math.Round(frameTime * 1000, 2),
                    Math.Round((unknown) * 1000, 2), RenderQueue.TrianglesDrawn, RenderDataPool.Count, RenderDataPool.Size, Math.Round(GuiRoot.Root.UpdateElapsedSeconds, 2), Math.Round(GuiRoot.Root.RebuildElapsedSeconds, 2), Math.Round(guiwait, 2), Math.Round(garbage, 2), Math.Round(guiRender, 2));
                frame = 0;
                update = 0;
            }

            text.Text = string.Format("Calculated weight: {0}", largeText.CalculatedWeight);

            // It is important that this is right before main render thread starts working on current context.
            Time.Render(e.Time);

            GL.Viewport(0, 0, Width, Height);

            GL.ClearColor(0.25f, 0.25f, 0.25f, 1);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            GL.Enable(EnableCap.DepthTest);

            base.OnRenderFrame(e);

            sw.Reset();
            sw.Start();
            RenderQueue.Render();
            sw.Stop();
            renderQueue = sw.Elapsed.TotalSeconds;

            sw.Reset();
            sw.Start();
            // Wait for gui update in case it was done in a background thread
            GuiRoot.Root.WaitUpdateFinish();
            sw.Stop();
            guiwait = sw.Elapsed.TotalSeconds;

            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            sw.Reset();
            sw.Start();
            GuiRoot.Root.Render();
            sw.Stop();
            guiRender = sw.Elapsed.TotalSeconds;

            

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

            GL.Flush();

            UserInput.FrameEnd();
        }

        int frame;
    }
}


