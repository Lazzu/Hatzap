using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hatzap.Gui.Widgets;
using Hatzap.Shaders;
using Hatzap.Textures;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Hatzap.Gui
{
    public class GuiRoot
    {
        public Matrix4 Projection = Matrix4.Identity;

        GameWindow gw;

        WidgetGroup widgets = new WidgetGroup();

        GuiRenderer renderer;

        Task updateTask;

        List<Widget> customRenderingWidgets = new List<Widget>();
        int customRenderingWidgetCount = 0;

        ShaderProgram shader;

        public TextureArray Texture { get; set; }

        internal GuiRoot(GameWindow gw)
        {
            Root = this;

            this.gw = gw;

            gw.Resize += gw_Resize;
            gw.Load += gw_Load;
            gw.Resize += gw_Resize;
            gw.KeyDown += gw_KeyDown;
            gw.KeyPress += gw_KeyPress;
            gw.KeyUp += gw_KeyUp;
            gw.MouseMove += gw_MouseMove;

            widgets.size = new Vector2(gw.Width, gw.Height);
        }

        void gw_MouseMove(object sender, OpenTK.Input.MouseMoveEventArgs e)
        {

        }

        void gw_Load(object sender, EventArgs e)
        {
            renderer = new GuiRenderer();
            shader = ShaderManager.Get("Gui");
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
            widgets.size = new Vector2(gw.Width, gw.Height);
        }

        Stopwatch sw = new Stopwatch();

        public double UpdateElapsedSeconds { get; protected set; }
        public double RebuildElapsedSeconds { get; protected set; }

        public void Update(double delta)
        {
            sw.Reset();
            sw.Start();
            UpdateGroup(widgets, delta);
            sw.Stop();
            UpdateElapsedSeconds = sw.Elapsed.TotalSeconds;

            sw.Reset();
            sw.Start();
            if (widgets.Dirty)
            {
                Rebuild();
            }
            sw.Stop();
            RebuildElapsedSeconds = sw.Elapsed.TotalSeconds;
        }

        void UpdateGroup(WidgetGroup rootGroup, double delta)
        {
            foreach (var widget in rootGroup.Widgets)
            {
                if (!widget.Visible)
                    continue;

                widget.Update(delta);

                if (widget.DirtyColor)
                    renderer.UpdateColor(widget.drawStartIndex, widget.drawEndIndex, widget.Color);

                if (widget.CustomRenderLayer != string.Empty)
                    EnqueueCustomRenderingWidget(widget);

                var group = widget as WidgetGroup;

                if (group != null)
                    UpdateGroup(group, delta);
            }
        }

        public void UpdateAsync(double delta)
        {
            updateTask = Task.Run(() => Update(delta));
        }

        public void Render()
        {
            if (Texture != null && shader != null)
            {
                if (renderer == null)
                    throw new Exception("No renderer initialized. Did you call GuiRoot.Initialize AFTER gamewindow.OnLoad's base.OnLoad()?");

                GL.Disable(EnableCap.DepthTest);

                shader.Enable();

                var textureSize = new Vector2(Texture.Width, Texture.Height);

                shader.SendUniform("Projection", ref Projection);
                shader.SendUniform("TextureSize", ref textureSize);

                Texture.Bind();
                renderer.Render();
                shader.Disable();

                
            }

            for (int i = 0; i < customRenderingWidgets.Count; i++)
            {
                var item = customRenderingWidgets[i];
                customRenderingWidgets[i] = null;
                
                if(item != null) item.CustomRender();
            }
            customRenderingWidgetCount = 0;
        }

        internal void EnqueueCustomRenderingWidget(Widget widget)
        {
            if(customRenderingWidgets.Count == customRenderingWidgetCount)
            {
                // No room in queue, add in the end of the queue
                customRenderingWidgets.Add(widget);
            }
            else
            {
                // Insert in queue
                customRenderingWidgets[customRenderingWidgetCount] = widget;
            }
            // Increase queue index
            customRenderingWidgetCount++;
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

        public void AddWidget(Widget widget)
        {
            widgets.AddChildWidget(widget);
        }

        public void RemoveWidget(Widget widget)
        {
            widgets.RemoveChildWidget(widget);
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
