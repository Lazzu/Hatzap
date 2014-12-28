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
        VertexBatch triangle;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            gl = new GLControl(new GraphicsMode(new ColorFormat(32)), 3, 3, GraphicsContextFlags.Default);

            gl.Paint += gl_Paint;
            gl.Resize += gl_Resize;
            gl.Load += gl_Load;

            winFormsHost.Child = gl;

            
        }

        void gl_Load(object sender, EventArgs e)
        {
            triangle = new VertexBatch();

            triangle.StartBatch(PrimitiveType.Triangles);

            triangle.Add(new Vector3(-1, -1, 0));
            triangle.Add(new Vector3(1, -1, 0));
            triangle.Add(new Vector3(0, 1, 0));

            triangle.EndBatch();
        }

        void gl_Resize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, gl.Width, gl.Height);
        }

        void gl_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            
            GL.ClearColor(0.25f, 0.25f, 0.25f, 1);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            triangle.Render();

            gl.SwapBuffers();
        }
    }
}
