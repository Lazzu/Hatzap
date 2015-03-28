using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using Assimp;
using Hatzap.Models;
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
        public static GLControl gl;
        EditorRenderer renderer;
        TextureManager textures;
        TextureSettingsWindow settingsDialog;

        BackgroundWorker backgroundWorker = new BackgroundWorker(){ WorkerReportsProgress = true, WorkerSupportsCancellation = true };
        ConcurrentQueue<Action<BackgroundWorker>> workQueue = new ConcurrentQueue<Action<BackgroundWorker>>();

        public MainWindow()
        {
            InitializeComponent();

            backgroundWorker.DoWork += backgroundWorker_DoWork;

            settingsDialog = new TextureSettingsWindow();
        }

        void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Action<BackgroundWorker> work;
            if(workQueue.TryDequeue(out work))
            {
                work(backgroundWorker);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            gl = new GLControl(new GraphicsMode(new ColorFormat(32), 24, 8, 16, new ColorFormat(0), 2, false), 3, 3, GraphicsContextFlags.Default);
            renderer = new EditorRenderer(gl);
            gl.Load += gl_Load;
            winFormsHost.Child = gl;
        }

        void gl_Load(object sender, EventArgs e)
        {
            textures = new TextureManager();
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
            backgroundWorker.CancelAsync();
            backgroundWorker = null;
            gl.Dispose();
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

        private void MenuImportTexture_Click(object sender, RoutedEventArgs e)
        {
            //workQueue.Enqueue((bw) =>
            //{
                // Configure open file dialog box
                Microsoft.Win32.OpenFileDialog openDlg = new Microsoft.Win32.OpenFileDialog();
                openDlg.FileName = ""; // Default file name
                //openDlg.DefaultExt = "*.png,*.jpg,*.bmp"; // Default file extension
                openDlg.Filter = "All supported texture files |*.png;*.jpg;*.bmp"; // Filter files by extension 

                // Show open file dialog box
                Nullable<bool> result = openDlg.ShowDialog();

                // Process open file dialog box results 
                if (result != true)
                    return;

                var filename = openDlg.FileName;

                var bmp = new Bitmap(filename);

                //bw.ReportProgress(33);

                settingsDialog = new TextureSettingsWindow();
                settingsDialog.TextureWidth = bmp.Width;
                settingsDialog.TextureHeight = bmp.Height;

                settingsDialog.ShowDialog();

                if (!settingsDialog.OKClicked)
                    return;

                var mipmapped = settingsDialog.Mipmapped;
                var preMipmapped = mipmapped && settingsDialog.MipmapsSaved;
                var compressed = settingsDialog.Compressed;
                var compressedSaved = compressed && settingsDialog.CompressionSaved;
                var format = (PixelInternalFormat)settingsDialog.PixelInternalFormat;
                var filtering = settingsDialog.TextureFiltering;
                var pixelFormat = OpenTK.Graphics.OpenGL.PixelFormat.Rgba;
                var anisolevel = settingsDialog.AnisoLevel;

                switch (format)
                {
                    case PixelInternalFormat.Rgba:
                        format = compressed ? PixelInternalFormat.CompressedRgbaS3tcDxt5Ext : format;
                        break;
                    case PixelInternalFormat.SrgbAlpha:
                        format = compressed ? PixelInternalFormat.CompressedSrgbAlphaS3tcDxt5Ext : format;
                        break;
                    case PixelInternalFormat.Rgb:
                        pixelFormat = OpenTK.Graphics.OpenGL.PixelFormat.Rgb;
                        format = compressed ? PixelInternalFormat.CompressedRgbS3tcDxt1Ext : format;
                        break;
                    case PixelInternalFormat.Srgb:
                        pixelFormat = OpenTK.Graphics.OpenGL.PixelFormat.Rgb;
                        format = compressed ? PixelInternalFormat.CompressedSrgbS3tcDxt1Ext : format;
                        break;
                    case PixelInternalFormat.Rg8:
                        pixelFormat = OpenTK.Graphics.OpenGL.PixelFormat.Rg;
                        format = compressed ? PixelInternalFormat.CompressedRg : format;
                        break;
                    case PixelInternalFormat.R8:
                        pixelFormat = OpenTK.Graphics.OpenGL.PixelFormat.Red;
                        format = compressed ? PixelInternalFormat.CompressedRedRgtc1 : format;
                        break;
                }

                // Load the texture temporarily in to GPU
                Texture texture = new Texture();

                texture.Quality = new TextureQuality()
                {
                    Filtering = filtering,
                    Mipmaps = mipmapped,
                    PregeneratedMipmaps = false,
                    TextureWrapMode = OpenTK.Graphics.OpenGL.TextureWrapMode.Clamp,
                    Anisotrophy = anisolevel,
                };

                // Set meta settings
                var meta = texture.Metadata;
                meta.PreMipmapped = false;
                meta.Precompressed = false;
                meta.PixelFormat = pixelFormat;
                meta.PixelInternalFormat = format;

                texture.Load(bmp, format);
                textures.Insert("tmp", texture);

                //bw.ReportProgress(33);

                // Apply settings (generate possible mipmaps etc)
                texture.UpdateQuality();
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
                GL.Finish();

                meta.PreMipmapped = preMipmapped;
                meta.Precompressed = compressedSaved;
                meta.Quality.PregeneratedMipmaps = preMipmapped;

                //bw.ReportProgress(33);

                filename = System.IO.Path.GetFileNameWithoutExtension(filename);
                filename += ".tex";

                // Configure save file dialog box
                Microsoft.Win32.SaveFileDialog saveDlg = new Microsoft.Win32.SaveFileDialog();
                saveDlg.FileName = filename; // Default file name
                saveDlg.DefaultExt = ".tex"; // Default file extension
                saveDlg.Filter = "Hatzap texture files|*.tex"; // Filter files by extension 

                // Show save file dialog box
                result = saveDlg.ShowDialog();

                // Process save file dialog box results 
                if (result == true)
                {
                    // Save document 
                    filename = saveDlg.FileName;

                    meta.FileName = filename;

                    textures.Save("tmp", filename);

                    texture.Release();
                    textures.Remove("tmp");
                }

                //bw.ReportProgress(1);

            //});

            //backgroundWorker.RunWorkerAsync();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            FreeResources();
        }
    }
}
