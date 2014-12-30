using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Hatzap;
using Hatzap.Models;
using Hatzap.Rendering;
using Hatzap.Shaders;
using Hatzap.Textures;
using Hatzap.Utilities;
using Hatzap_Editor.Renderables;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Hatzap_Editor
{
    class EditorRenderer
    {
        public static int Width { get; protected set; }
        public static int Height { get; protected set; }

        public static EditorRenderer Current { get; set; }


        GLControl glc;
        RenderQueue renderQueue;
        Camera camera;

        public RenderableAsset Asset { get; set; }

        public EditorRenderer(GLControl ctrl)
        {
            if (Current != null)
                throw new Exception("EditorRenderer already exists!");

            Current = this;

            glc = ctrl;

            ctrl.Load += glc_Load;
            ctrl.Resize += glc_Resize;
            ctrl.Paint += glc_Paint;
        }

        public void Redraw()
        {
            glc.Invalidate();
        }

        void glc_Load(object sender, EventArgs e)
        {
            // Load shaders
            var collection = XML.Read.FromFile<ShaderCollection>("EditorAssets/Shaders/collection.xml");
            ShaderManager.LoadCollection(collection);

            renderQueue = new RenderQueue();

            camera = new Camera(null);
            camera.SetAsCurrent();

            GL.Enable(EnableCap.Dither);
        }

        void glc_Resize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, glc.Width, glc.Height);
            Width = glc.Width;
            Height = glc.Height;
            /*float w = Width / 2.0f;
            float h = Height / 2.0f;
            camera.Projection = Matrix4.CreateOrthographicOffCenter(-w, w, -h, h, -1, 1);*/
        }
        
        void glc_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            PrepareRendering();

            GL.ClearColor(0.25f, 0.25f, 0.25f, 1);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Camera.Current.Update(0);

            renderQueue.Render();
            
            glc.SwapBuffers();
        }

        private void PrepareRendering()
        {
            if(Asset != null)
            {
                renderQueue.Insert(Asset.Renderable);
            }
        }
    }
}
