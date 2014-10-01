using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hatzap.Gui.Widgets;
using OpenTK;

namespace Hatzap.Gui
{
    public class GuiRoot
    {
        public Matrix4 Projection = Matrix4.Identity;

        GameWindow gw;

        WidgetGroup widgets = new WidgetGroup();

        GuiRenderer renderer;

        Task updateTask;

        internal GuiRoot(GameWindow gw)
        {
            Root = this;

            this.gw = gw;

            gw.Load += gw_Load;
            gw.Resize += gw_Resize;
            gw.KeyDown += gw_KeyDown;
            gw.KeyPress += gw_KeyPress;
            gw.KeyUp += gw_KeyUp;
            gw.MouseMove += gw_MouseMove;
        }

        void gw_MouseMove(object sender, OpenTK.Input.MouseMoveEventArgs e)
        {

        }

        void gw_Load(object sender, EventArgs e)
        {
            renderer = new GuiRenderer();
        }

        void gw_KeyUp(object sender, OpenTK.Input.KeyboardKeyEventArgs e)
        {
            
        }

        void gw_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        void gw_KeyDown(object sender, OpenTK.Input.KeyboardKeyEventArgs e)
        {
            
        }

        void gw_Resize(object sender, EventArgs e)
        {
            Projection = Matrix4.CreateOrthographicOffCenter(0, gw.Width, gw.Height, 0, -1, 1);
        }

        public void Update(double delta)
        {
            if (widgets.Dirty)
            {
                Rebuild();
            }
        }

        public void UpdateAsync(double delta)
        {
            updateTask = Task.Run(() => Update(delta));
        }

        public void Render()
        {
            renderer.Render();
        }

        async void Rebuild()
        {
            renderer.Build(widgets);
        }

        public void WaitUpdateFinish()
        {
            if(updateTask != null && !updateTask.IsCompleted)
            {
                updateTask.Wait();
            }
        }

        public static GuiRoot Root;
        public static void Initialize(GameWindow gw)
        {
            if (Root != null)
                return;

            Root = new GuiRoot(gw);
        }
    }
}
