using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hatzap.Gui.Events;
using Hatzap.Gui.Widgets;
using Hatzap.Shaders;
using Hatzap.Textures;
using Hatzap.Utilities;
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
            Projection = Matrix4.CreateOrthographicOffCenter(0, gw.Width, gw.Height, 0, -100, 100);
            widgets.Size = new Vector2(gw.Width, gw.Height);
            if (widgets.ChildWidgetCount > 0)
                widgets.Dirty = true;
        }

        Stopwatch sw = new Stopwatch();

        public double UpdateElapsedSeconds { get; protected set; }
        public double RebuildElapsedSeconds { get; protected set; }

        public void Update(double delta)
        {
            InternalUpdate(delta);
            HandleEvents();
        }

        void InternalUpdate(double delta)
        {
            Time.StartTimer("GuiRoot.InternalUpdate()", "Update");

            MouseOverGuiElementLastFrame = MouseOverGuiElement;
            MouseOverGuiElement = false;
            
            UpdateGroup(widgets, delta);

            if (widgets.Dirty)
            {
                Time.StartTimer("GuiRoot.Rebuild()", "Update");
                Rebuild();
                Time.StopTimer("GuiRoot.Rebuild()");
            }

            Time.StopTimer("GuiRoot.InternalUpdate()");
        }

        private void HandleEvents()
        {
            GuiEventManager.Current.HandleEvents();
        }

        void UpdateGroup(WidgetGroup rootGroup, double delta)
        {
            for (int i = 0; i < rootGroup.Widgets.Count; i++)
            {
                var widget = rootGroup.Widgets[i];

                if (!widget.Visible)
                    continue;

                widget.Update(delta);

                if (widget.DirtyColor)
                {
                    renderer.UpdateColor(widget.drawStartIndex, widget.drawEndIndex, widget.Color);
                    widget.DirtyColor = false;
                    //Debug.WriteLine("DirtyColor = " + widget);
                }

                if (widget.CustomRenderLayer != string.Empty)
                    EnqueueCustomRenderingWidget(widget);

                var group = widget as WidgetGroup;

                if (group != null) UpdateGroup(group, delta);
            }

            if (rootGroup.RequiresSorting)
            {
                rootGroup.SortChildWidgets();
                rootGroup.RequiresSorting = false;
            }
        }

        public void UpdateAsync(double delta)
        {
            Time.StartTimer("GuiRoot.UpdateAsync()", "Update");
            updateTask = Task.Run(() =>
            {
                InternalUpdate(delta);
            });
            Time.StopTimer("GuiRoot.UpdateAsync()");
        }

        public void Render()
        {
            Time.StartTimer("GuiRoot.Render()", "Render");

            if (Texture != null && shader != null)
            {
                if (renderer == null)
                    throw new Exception("No renderer initialized. Did you call GuiRoot.Initialize AFTER gamewindow.OnLoad's base.OnLoad()?");

                GLState.DepthTest = false;

                shader.Enable();

                var textureSize = new Vector2(Texture.Width, Texture.Height);

                shader.SendUniform("Projection", ref Projection);
                shader.SendUniform("TextureSize", ref textureSize);

                Texture.Bind();
                renderer.Render();
                shader.Disable();
            }

            Time.StartTimer("GuiRoot.CustomRender()", "Render");
            for (int i = 0; i < customRenderingWidgetCount; i++)
            {
                var item = customRenderingWidgets[i];
                customRenderingWidgets[i] = null;
                
                if(item != null) item.CustomRender();
            }
            customRenderingWidgetCount = 0;
            Time.StopTimer("GuiRoot.CustomRender()");
            Time.StopTimer("GuiRoot.Render()");
        }

        internal void EnqueueCustomRenderingWidget(Widget widget)
        {
            lock(customRenderingWidgets)
            {
                if (customRenderingWidgets.Count <= customRenderingWidgetCount)
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
        }

        async void Rebuild()
        {
            renderer.Build(widgets);
        }

        public void WaitUpdateFinish()
        {
            Time.StartTimer("GuiRoot.WaitUpdateFinish()", "Update");
            if(updateTask != null && !updateTask.IsCompleted)
            {
                updateTask.Wait();
            }
            HandleEvents();
            Time.StopTimer("GuiRoot.WaitUpdateFinish()");
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

        /// <summary>
        /// Not realiable when using threaded gui update. If you want to be sure, use MouseOverGuiElementLastFrame in addition to MouseOverGuiElement to get better results.
        /// </summary>
        public bool MouseOverGuiElement { get; internal set; }

        /// <summary>
        /// True if mouse was over gui element on last frame.
        /// </summary>
        public bool MouseOverGuiElementLastFrame { get; internal set; }

        /// <summary>
        /// Short for testing if either MouseOverGuiElement or MouseOverGuiElementLastFrame is true.
        /// </summary>
        public bool MouseHoverGui { get { return MouseOverGuiElement || MouseOverGuiElementLastFrame; } }
    }
}
