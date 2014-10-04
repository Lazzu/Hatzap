using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hatzap.Input;
using OpenTK;
using OpenTK.Input;

namespace Hatzap.Gui.Widgets
{
    public abstract class Widget
    {
        /// <summary>
        /// Widget position on screen.
        /// </summary>
        public Vector2 Position;

        /// <summary>
        /// Widget size on screen.
        /// </summary>
        public Vector2 Size;

        /// <summary>
        /// The widget texture region(s). Some widgets require more regions than others. Check documentation for each widget invidually.
        /// </summary>
        public GuiTextureRegion[] TextureRegion;
        
        // Should only be accessed by property
        private int z;
        private bool dirty;
        private bool visible = true;

        /// <summary>
        /// The depth-sorting index in the current widget group.
        /// </summary>
        public int Z { get { return z; } set { z = value; Dirty = true;  if (WidgetGroup != null) WidgetGroup.SortChildWidgets(); } }

        /// <summary>
        /// The widget group this widget belongs to.
        /// </summary>
        public WidgetGroup WidgetGroup { get; set; }

        /// <summary>
        /// Gets if the widget is the currently active one.
        /// </summary>
        public bool Active { get { return this == Widget.CurrentlyActive; } }

        /// <summary>
        /// If set to true, this widget requires updating
        /// </summary>
        public bool Dirty { get { return dirty; } set { dirty = value; if (dirty && WidgetGroup != null) WidgetGroup.Dirty = dirty; } }

        /// <summary>
        /// Gets or sets if the widget is visible.
        /// </summary>
        public bool Visible { get { return visible; } set { visible = value; Dirty = true; } }

        #region Event functions

        #region MouseEvents
        public virtual void OnMouseEnter() { }
        public virtual void OnMouseLeave() { }
        public virtual void OnMouseHover() { }
        public virtual void OnMouseClick(MouseButton button) { }
        public virtual void OnMouseDown(MouseButton button) { }
        public virtual void OnMouseUp(MouseButton button) { }
        #endregion

        #region KeyboardEvents
        public virtual void OnKeyPress(Key key) { }
        public virtual void OnKeyDown(Key key) { }
        public virtual void OnKeyUp(Key key) { }
        #endregion

        #endregion

        /// <summary>
        /// The update function gets called on every frame update.
        /// </summary>
        /// <param name="delta">The delta time of the current frame.</param>
        public void Update(double delta) 
        {
            if (!Visible)
                return;

            if (UserInput.Mouse.IsInsideRect(Position, Position + Size) && UserInput.Mouse.IsButtonClicked())
            {
                var buttons = UserInput.Mouse.GetClickedButtons();
                foreach (var button in buttons)
                {
                    OnMouseClick(button);
                }
            }

            OnUpdate(delta);
        }

        public virtual void OnUpdate(double delta) { }

        /// <summary>
        /// Returns all
        /// </summary>
        /// <returns></returns>
        public virtual GuiVertex[] GetVertices() { return null; }

        /// <summary>
        /// This function is used for rendering custom GUI stuff after all other GUI rendering is done.
        /// Useful for displaying custom objects like textured sprite or text.
        /// </summary>
        /// <param name="delta">The delta time of the current frame.</param>
        public virtual void CustomRender() { }

        /// <summary>
        /// The layer for ordering the custom rendering calls, to minimize shader bindings and GL calls. 
        /// If returns string.Empty, it indicates that this object has no need for custom rendering pass.
        /// </summary>
        public virtual string CustomRenderLayer { get { return string.Empty; } }

        /// <summary>
        /// Gets the currently active widget. Used internally for calling user input event functions, but is exposed as public for convenience.
        /// </summary>
        public static Widget CurrentlyActive { get; internal set; }

    }
}
