using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using Assimp;
using Hatzap.Models;
using Hatzap.Rendering;
using Hatzap.Textures;
using Hatzap_Editor.ContentProcessors.Mesh;
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
                    TextureWrapMode = OpenTK.Graphics.OpenGL.TextureWrapMode.Clamp
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

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            FreeResources();

            this.Close();
        }

        private void FreeResources()
        {
            
        }

        private void MenuImportMesh_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog openDlg = new Microsoft.Win32.OpenFileDialog();
            openDlg.FileName = ""; // Default file name
            openDlg.DefaultExt = ".*"; // Default file extension
            openDlg.Filter = "All files |*.*"; // Filter files by extension 

            // Show open file dialog box
            Nullable<bool> result = openDlg.ShowDialog();

            // Process open file dialog box results 
            if (result == true)
            {
                // Open document 
                string filename = openDlg.FileName;
                
                //Create a new importer
                AssimpContext importer = new AssimpContext();

                //This is how we add a configuration (each config is its own class)
                //NormalSmoothingAngleConfig config = new NormalSmoothingAngleConfig(66.0f);
                //importer.SetConfig(config);

                var flags = PostProcessPreset.TargetRealTimeMaximumQuality | PostProcessSteps.Triangulate | PostProcessSteps.SortByPrimitiveType | PostProcessSteps.FlipUVs;

                //Import the model. All configs are set. The model
                //is imported, loaded into managed memory. Then the unmanaged memory is released, and everything is reset.
                Scene model = importer.ImportFile(filename, flags);

                var mesh = new Hatzap.Models.Mesh();

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
                    AssimpConvertor ac = new AssimpConvertor();

                    mesh = ac.FromAssimp(aMesh);
                }
                else
                {
                    Debug.WriteLine("ERROR: No triangle meshes found in imported model.");
                }

                importer.Dispose();

                if (mesh == null)
                    return;

                filename = System.IO.Path.GetFileNameWithoutExtension(filename);

                filename += ".mesh";

                // Configure save file dialog box
                Microsoft.Win32.SaveFileDialog saveDlg = new Microsoft.Win32.SaveFileDialog();
                saveDlg.FileName = filename; // Default file name
                saveDlg.DefaultExt = ".mesh"; // Default file extension
                saveDlg.Filter = "Mesh files (.mesh)|*.mesh"; // Filter files by extension 

                // Show save file dialog box
                result = saveDlg.ShowDialog();

                // Process save file dialog box results 
                if (result == true)
                {
                    // Save document 
                    filename = saveDlg.FileName;

                    MeshManager meshManager = new MeshManager();

                    using (var fs = File.OpenWrite(filename))
                    {
                        meshManager.TemporarySaveAsset(mesh, fs);
                    }
                }
            }
        }
    }
}
