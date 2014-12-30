using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Hatzap.Rendering;
using Hatzap.Textures;
using Hatzap_Editor.Renderables;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Hatzap_Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GLControl gl;
        EditorRenderer renderer;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            gl = new GLControl(new GraphicsMode(new ColorFormat(32), 24, 8, 16, new ColorFormat(0), 2, false), 3, 3, GraphicsContextFlags.Default);
            renderer = new EditorRenderer(gl);
            winFormsHost.Child = gl;
        }

        private void LoadTexture_Click(object sender, RoutedEventArgs e)
        {
            TextureMeta textureMeta = new TextureMeta()
            {
                FileName = "EditorAssets/Textures/test.png",
                PixelInternalFormat = PixelInternalFormat.Rgba,
                PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                PixelType = PixelType.UnsignedByte,
                Quality = new TextureQuality()
                {
                    Filtering = TextureFiltering.Trilinear,
                    Anisotrophy = 16,
                    Mipmaps = true,
                    TextureWrapMode = TextureWrapMode.Clamp
                }
            };

            var t = new Texture();
            t.Load(textureMeta);

            RenderableTexture texture = new RenderableTexture(t);
            renderer.Asset = texture;

            renderer.Redraw();
        }

        void OpenTab(string header, RenderableAsset asset)
        {
            gl = new GLControl(new GraphicsMode(new ColorFormat(32), 24, 8, 16, new ColorFormat(0), 2, false), 3, 3, GraphicsContextFlags.Default);
            renderer = new EditorRenderer(gl);
            winFormsHost.Child = gl;
        }
    }
}
