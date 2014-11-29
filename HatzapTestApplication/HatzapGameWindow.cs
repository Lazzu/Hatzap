using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Assimp;
using Assimp.Configs;
using Awesomium.Core;
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

        ConcurrentQueue<Vector4> subAreas = new ConcurrentQueue<Vector4>();

        private void UploadWebpageToGPU(WebView webView)
        {
            var surface = (BitmapSurface)webView.Surface;

            surface.Updated += surface_Updated;

            subAreas.Enqueue(new Vector4(0, 0, Width, Height));

            // A BitmapSurface is assigned by default to all WebViews.
            uploadableBuffer = surface.Buffer;
        }

        void surface_Updated(object sender, SurfaceUpdatedEventArgs e)
        {
            subAreas.Enqueue(new Vector4(e.DirtyRegion.X, e.DirtyRegion.Y, e.DirtyRegion.Width, e.DirtyRegion.Height));
            finishedLoading = true;
        }

        private void UploadUpdatedPixels()
        {
            if (subAreas.Count == 0)
                return;
            
            lock (WebCore.SyncRoot)
            {
                Vector4 subArea;
                while (subAreas.TryDequeue(out subArea))
                {
                    int x = (int)subArea.X;
                    int y = (int)subArea.Y;
                    int w = (int)subArea.Z;
                    int h = (int)subArea.W;

                    IntPtr newPtr = uploadableBuffer + (x + y * Width) * sizeof(Int32);

                    streamedTexture.UploadRegion(newPtr, x, y, w, h);
                }
            }

        }

        protected override void OnLoad(EventArgs e)
        {
            Debug.WriteLine("OnLoad()");

            thread = new Thread(new ThreadStart(() =>
            {
                view = WebCore.CreateWebView(Width, Height, WebViewType.Offscreen);

                //WebCore.AutoUpdatePeriod = 30;

                view.IsTransparent = true;

                finishedLoading = false;

                var path = Path.Combine(new String[] {
                    Directory.GetCurrentDirectory(),
                    "TestWebPages\\index.html"
                });

                path = "http://lasoft.fi/";

                // Load some content.
                view.Source = new Uri(path);

                // Handle the LoadingFrameComplete event.
                // For this example, we use a lambda expression.
                view.LoadingFrameComplete += (s, eargs) =>
                {
                    if (!eargs.IsMainFrame)
                        return;

                    UploadWebpageToGPU((WebView)s);
                    finishedLoading = true;
                };

                if (WebCore.UpdateState == WebCoreUpdateState.NotUpdating)
                    WebCore.Run();

            }));

            thread.Start();

            GPUCapabilities.Initialize();

            Debug.WriteLine("GPUCapabilities.Version=" + GPUCapabilities.Version);
            Debug.WriteLine("GPUCapabilities.GLSL=" + GPUCapabilities.GLSL);
            Debug.WriteLine("GPUCapabilities.Instancing=" + GPUCapabilities.Instancing);
            Debug.WriteLine("GPUCapabilities.MaxVaryingFloats=" + GPUCapabilities.MaxVaryingFloats);
            Debug.WriteLine("GPUCapabilities.MaxVaryingVectors=" + GPUCapabilities.MaxVaryingVectors);
            Debug.WriteLine("GPUCapabilities.SeamlessCubemaps=" + GPUCapabilities.SeamlessCubemaps);

            if (GPUCapabilities.SeamlessCubemaps)
                GL.Enable(EnableCap.TextureCubeMapSeamless);

            GLState.DepthTest = true;
            GLState.AlphaBleding = true;
            GLState.CullFace = true;

            SceneManager.Initialize(500, 5, 20, Vector3.Zero);
            SceneManager.CullByObject = false;

            RenderQueue.AllowInstancing = true;

            viewPort = new Vector2(Width, Height);

            camera = new Hatzap.Camera(this);
            
            camera.SetAsCurrent();

            camera.Position = new Vector3(0, 1, 1);
            camera.Target = new Vector3(-1, 0, 0);

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

            //Console.WriteLine("Test test ABCDEFGHIJKLMNOPQRSTUVWXYZÅÄÖ");

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
                    Path = "Assets/Shaders/SimpleModel2.vert",
                    Type = ShaderType.VertexShader
                },new ShaderInfo() {
                    Path = "Assets/Shaders/SimpleModel.frag",
                    Type = ShaderType.FragmentShader
                },/*new ShaderInfo() {
                    Path = "Assets/Shaders/SimpleModel.geom",
                    Type = ShaderType.GeometryShader
                }*/}),
                Name = "SimpleModel"
            });

            collection.ShaderPrograms.Add(new ShaderProgramInfo()
            {
                Shaders = new List<ShaderInfo>(new[]{ new ShaderInfo() {
                    Path = "Assets/Shaders/SimpleModel2.vert",
                    Type = ShaderType.VertexShader
                },new ShaderInfo() {
                    Path = "Assets/Shaders/SimpleModel1.frag",
                    Type = ShaderType.FragmentShader
                },/*new ShaderInfo() {
                    Path = "Assets/Shaders/SimpleModel.geom",
                    Type = ShaderType.GeometryShader
                }*/}),
                Name = "Textureless"
            });

            //XML.Write.ToFile(collection, "Assets/Shaders/collection.xml");

            ShaderManager.LoadCollection(collection);

            Time.Initialize();
            UserInput.Initialize(this, typeof(AccurateMouse), typeof(Hatzap.Input.Keyboard));
            UserInput.Mouse.ClickInterval = 0.5f;
            GuiRoot.Initialize(this);

            GuiRoot.Root.Texture = new TextureArray();

            GuiRoot.Root.Texture.Load(new[] { new Bitmap("Assets/Textures/greySheet.png") }, PixelInternalFormat.Rgba, PixelFormat.Bgra, PixelType.UnsignedByte);

            GuiRoot.Root.Texture.TextureSettings(TextureMinFilter.Nearest, TextureMagFilter.Nearest, 0);

            ElementCollection guiElements = new ElementCollection()
            {
                Elements = new List<WidgetInfo> { 
                    new WidgetInfo(){
                        WidgetType = typeof(Button).ToString(),
                        Slices = new List<GuiTextureRegion> {
                            new GuiTextureRegion() { // Top left
                                Offset = new Vector2(49,433),
                                Size = new Vector2(6,4),
                                Page = 0
                            }, new GuiTextureRegion() { // Top center
                                Offset = new Vector2(55,433),
                                Size = new Vector2(37,4),
                                Page = 0
                            }, new GuiTextureRegion() { // Top Right
                                Offset = new Vector2(92,433),
                                Size = new Vector2(6,4),
                                Page = 0
                            }, new GuiTextureRegion() { // Middle left
                                Offset = new Vector2(49,437),
                                Size = new Vector2(6,36),
                                Page = 0
                            }, new GuiTextureRegion() { // Middle center
                                Offset = new Vector2(55,437),
                                Size = new Vector2(37,36),
                                Page = 0
                            }, new GuiTextureRegion() { // Middle right
                                Offset = new Vector2(92,437),
                                Size = new Vector2(6,36),
                                Page = 0
                            }, new GuiTextureRegion() { // Bottom left
                                Offset = new Vector2(49,473),
                                Size = new Vector2(6,5),
                                Page = 0
                            }, new GuiTextureRegion() { // Bottom center
                                Offset = new Vector2(55,473),
                                Size = new Vector2(37,5),
                                Page = 0
                            }, new GuiTextureRegion() { // Bottom right
                                Offset = new Vector2(92,473),
                                Size = new Vector2(6,5),
                                Page = 0
                            },
                        },
                    },
                    new WidgetInfo(){
                        WidgetType = typeof(Window).ToString(),
                        Slices = new List<GuiTextureRegion> {
                             new GuiTextureRegion() { // Top left
                                Offset = new Vector2(190,98),
                                Size = new Vector2(7,5),
                                Page = 0
                            }, new GuiTextureRegion() { // Top center
                                Offset = new Vector2(195,98),
                                Size = new Vector2(86,5),
                                Page = 0
                            }, new GuiTextureRegion() { // Top Right
                                Offset = new Vector2(283,98),
                                Size = new Vector2(7,5),
                                Page = 0
                            }, new GuiTextureRegion() { // Middle left
                                Offset = new Vector2(190,103),
                                Size = new Vector2(7,89),
                                Page = 0
                            }, new GuiTextureRegion() { // Middle center
                                Offset = new Vector2(197,103),
                                Size = new Vector2(86,89),
                                Page = 0
                            }, new GuiTextureRegion() { // Middle right
                                Offset = new Vector2(283,103),
                                Size = new Vector2(7,89),
                                Page = 0
                            }, new GuiTextureRegion() { // Middle left
                                Offset = new Vector2(190,103),
                                Size = new Vector2(7,89),
                                Page = 0
                            }, new GuiTextureRegion() { // Middle center
                                Offset = new Vector2(197,103),
                                Size = new Vector2(86,89),
                                Page = 0
                            }, new GuiTextureRegion() { // Middle right
                                Offset = new Vector2(283,103),
                                Size = new Vector2(7,89),
                                Page = 0
                            }, new GuiTextureRegion() { // Bottom left
                                Offset = new Vector2(190,192),
                                Size = new Vector2(7,6),
                                Page = 0
                            }, new GuiTextureRegion() { // Bottom center
                                Offset = new Vector2(197,192),
                                Size = new Vector2(86,6),
                                Page = 0
                            }, new GuiTextureRegion() { // Bottom right
                                Offset = new Vector2(283,192),
                                Size = new Vector2(7,6),
                                Page = 0
                            }
                        },
                    },
                    new WidgetInfo(){
                        WidgetType = typeof(Panel).ToString(),
                        Slices = new List<GuiTextureRegion> {
                            new GuiTextureRegion() { // Top left
                                Offset = new Vector2(190,98),
                                Size = new Vector2(7,5),
                                Page = 0
                            }, new GuiTextureRegion() { // Top center
                                Offset = new Vector2(195,98),
                                Size = new Vector2(86,5),
                                Page = 0
                            }, new GuiTextureRegion() { // Top Right
                                Offset = new Vector2(283,98),
                                Size = new Vector2(7,5),
                                Page = 0
                            }, new GuiTextureRegion() { // Middle left
                                Offset = new Vector2(190,103),
                                Size = new Vector2(7,89),
                                Page = 0
                            }, new GuiTextureRegion() { // Middle center
                                Offset = new Vector2(197,103),
                                Size = new Vector2(86,89),
                                Page = 0
                            }, new GuiTextureRegion() { // Middle right
                                Offset = new Vector2(283,103),
                                Size = new Vector2(7,89),
                                Page = 0
                            }, new GuiTextureRegion() { // Bottom left
                                Offset = new Vector2(190,192),
                                Size = new Vector2(7,6),
                                Page = 0
                            }, new GuiTextureRegion() { // Bottom center
                                Offset = new Vector2(197,192),
                                Size = new Vector2(86,6),
                                Page = 0
                            }, new GuiTextureRegion() { // Bottom right
                                Offset = new Vector2(283,192),
                                Size = new Vector2(7,6),
                                Page = 0
                            }
                        },
                    },
                }
            };

            XML.Write.ToFile(guiElements, "Assets/Gui/elements.xml");

            GridContainer mainGuiGrid = new GridContainer();
            mainGuiGrid.Columns = 1;
            mainGuiGrid.Rows = 3;
            mainGuiGrid.CellWidths.Add(1280);
            mainGuiGrid.RowHeights.Add(30);
            mainGuiGrid.RowHeights.Add(0);
            mainGuiGrid.RowHeights.Add(30);
            mainGuiGrid.Anchor = new Anchor();
            mainGuiGrid.Anchor.Directions[AnchorDirection.Top] = AnchorType.Snap;
            mainGuiGrid.Anchor.Directions[AnchorDirection.Left] = AnchorType.Snap;
            mainGuiGrid.Anchor.Directions[AnchorDirection.Right] = AnchorType.Snap;
            mainGuiGrid.Anchor.Directions[AnchorDirection.Bottom] = AnchorType.Snap;
            mainGuiGrid.Position = new Vector2(0, 0);
            mainGuiGrid.Size = new Vector2(400, 200);

            Panel leftPanel = new Panel();
            leftPanel.Anchor = new Anchor();
            leftPanel.Anchor.Directions[AnchorDirection.Left] = AnchorType.Snap;
            leftPanel.Anchor.Directions[AnchorDirection.Top] = AnchorType.Snap;
            leftPanel.Anchor.Directions[AnchorDirection.Bottom] = AnchorType.Snap;

            leftPanel.Position = new Vector2(100, 100);
            leftPanel.Size = new Vector2(300, 10);
            leftPanel.Color = new Vector4(0.1f, 0.1f, 0.1f, 1f);
            leftPanel.RightAnchorOffset = -10.0f;

            leftPanel.TextureRegion = guiElements.GetInfo(leftPanel).Slices.ToArray();

            Panel menuBar = new Panel();
            menuBar.Anchor = new Anchor();
            menuBar.Anchor.Directions[AnchorDirection.Top] = AnchorType.Snap;
            menuBar.Anchor.Directions[AnchorDirection.Left] = AnchorType.Snap;
            menuBar.Anchor.Directions[AnchorDirection.Right] = AnchorType.Snap;

            menuBar.Position = new Vector2(100, 100);
            menuBar.Size = new Vector2(300, 30);
            menuBar.Color = new Vector4(0.1f, 0.1f, 0.1f, 1f);

            menuBar.TextureRegion = guiElements.GetInfo(leftPanel).Slices.ToArray();

            Panel bottomBar = new Panel();
            bottomBar.Anchor = new Anchor();
            bottomBar.Anchor.Directions[AnchorDirection.Left] = AnchorType.Snap;
            bottomBar.Anchor.Directions[AnchorDirection.Right] = AnchorType.Snap;
            bottomBar.Anchor.Directions[AnchorDirection.Bottom] = AnchorType.Snap;

            bottomBar.Position = new Vector2(0, 690);
            bottomBar.Size = new Vector2(0, 30);
            bottomBar.Color = new Vector4(0.1f, 0.1f, 0.1f, 1f);

            bottomBar.TextureRegion = guiElements.GetInfo(leftPanel).Slices.ToArray();

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

            stack.Anchor = new Anchor();
            stack.Anchor.Directions[AnchorDirection.Left] = AnchorType.Snap;
            stack.Anchor.Directions[AnchorDirection.Top] = AnchorType.Snap;
            stack.Anchor.Directions[AnchorDirection.Right] = AnchorType.Snap;
            stack.Anchor.Directions[AnchorDirection.Bottom] = AnchorType.Snap;

            stack.RightAnchorOffset = 5;
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

            leftPanel.AddChildWidget(stack);

            mainGuiGrid.AddChildWidget(menuBar);
            mainGuiGrid.AddChildWidget(leftPanel);
            mainGuiGrid.AddChildWidget(bottomBar);

            GuiRoot.Root.AddWidget(mainGuiGrid);
            
            
            

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
            Scene model = importer.ImportFile("Assets/Models/suzanne.fbx", flags);
            
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

            int n = 1;
            int sizeScale = 5;

            Random rand = new Random();

            for (int x = -n; x <= n; x++)
            {
                for (int y = -n; y <= n; y++)
                {
                    Hatzap.Models.Material spaceShipMaterial = new Hatzap.Models.Material();

                    spaceShipMaterial.UniformData = new List<IUniformData> { 
                        new UniformDataVector4()
                        {
                            Name = "Color",
                            Data = new Vector4((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble(), 1.0f)
                        }
                    };

                    for (int z = -n; z <= n; z++)
                    {
                        var spaceShip = new Model();
                        spaceShip.Texture = shipTexture;
                        spaceShip.Shader = ShaderManager.Get("Textureless");
                        spaceShip.Mesh = mesh;
                        spaceShip.Transform.Static = true;
                        spaceShip.Transform.Position = new Vector3(x * sizeScale, y * sizeScale, z * sizeScale);
                        spaceShip.Transform.Rotation = Quaternion.FromEulerAngles(x * 360.0f / n / (float)Math.PI, y * 360.0f / n / (float)Math.PI, z * 360.0f / n / (float)Math.PI);

                        spaceShip.Material = spaceShipMaterial;

                        SceneManager.Insert(spaceShip);
                    }
                }
            }

            streamedTexture = new Texture(Width, Height);
            streamedTexture.Generate(PixelFormat.Bgra, PixelType.Byte, TextureMinFilter.Nearest, TextureMagFilter.Nearest, 0);

            Debug.WriteLine("OnLoad() ends");
            
            base.OnLoad(e);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);

            WebCore.Shutdown();
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
        
        protected override void OnUpdateFrame(OpenTK.FrameEventArgs e)
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

            camera.Perspective(Width, Height, 30, 1.0f, 80.0f);
            camera.Update((float)e.Time);

            update++;
            
            //camera.Rotate(new Vector2(0, (mousepos.X / (float)Width - 0.5f) * 0.5f * (float)e.Time));
            camera.Rotate(new Vector2(0,(float)e.Time * 0.1f));
            camera.Distance = (float)(Math.Sin(totalTime * 0.25f) + 1.2f) * 10;

            Time.StopTimer("UpdateFrame()");
        }
        
        protected override void OnRenderFrame(OpenTK.FrameEventArgs e)
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

            //GuiRoot.Root.Render();

            UploadUpdatedPixels();
            streamedTexture.Bind();
            ScreenFiller.DrawScreenFillingTexturedQuad();
            streamedTexture.UnBind();

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
        private Texture streamedTexture;
        private Thread thread;
        private WebView view;
        private bool finishedLoading;
        private IntPtr uploadableBuffer;
    }
}


