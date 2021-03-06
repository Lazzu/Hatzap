﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hatzap.Gui.Anchors;
using Hatzap.Gui.Events;
using Hatzap.Input;
using Hatzap.Utilities;
using OpenTK;
using OpenTK.Input;

namespace Hatzap.Gui.Widgets
{
    public abstract class Widget
    {
        // Should only be accessed by property
        internal Vector2 position;
        internal Vector2 size;
        private int z;
        private bool dirty;
        private bool visible = true;
        private Anchor anchor;
        private Vector4 color = Vector4.One;


        /// <summary>
        /// Widget position on screen.
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        /// <summary>
        /// Widget size on screen.
        /// </summary>
        public Vector2 Size
        {
            get { return size; }
            set { size = value; }
        }

        /// <summary>
        /// Color tinting of the Widget. result = texture.rgba * widget.Color;
        /// </summary>
        public Vector4 Color
        {
            get { return color; }
            set { color = value; DirtyColor = true; }
        }

        /// <summary>
        /// The widget texture region(s). Some widgets require more regions than others. Check documentation for each widget invidually.
        /// </summary>
        public GuiTextureRegion[] TextureRegion;
        
        /// <summary>
        /// The first index of vertex array that this widget produced. Should not be touched by the widget itself.
        /// </summary>
        internal int drawStartIndex;

        /// <summary>
        /// The last index of vertex array that this widget produced. Should not be touched by the widget itself.
        /// </summary>
        internal int drawEndIndex;

        /// <summary>
        /// Widget's anchor. If null, no anchoring.
        /// </summary>
        public Anchor Anchor
        {
            get { return anchor; }
            set { anchor = value; anchor.Parent = this; }
        }

        /// <summary>
        /// The depth-sorting index in the current widget group.
        /// </summary>
        public int Z
        {
            get { return z; }
            set { z = value; Dirty = true; if (WidgetGroup != null) WidgetGroup.RequiresSorting = true; }
        }

        /// <summary>
        /// The widget group this widget belongs to.
        /// </summary>
        public WidgetGroup WidgetGroup { get; set; }

        /// <summary>
        /// Gets if the widget is the currently active one.
        /// </summary>
        public bool Active
        {
            get { return this == Widget.CurrentlyActive; }
        }

        /// <summary>
        /// If set to true, this widget requires updating
        /// </summary>
        public bool Dirty
        {
            get { return dirty; }
            set
            {
                dirty = value;
                if (dirty)
                    if (WidgetGroup != null)
                        WidgetGroup.Dirty = dirty;
            }
        }

        /// <summary>
        /// Indicates that the color has been changed, and the internal buffer needs updating.
        /// </summary>
        public bool DirtyColor { get; set; }

        /// <summary>
        /// Gets or sets if the widget is visible.
        /// </summary>
        public bool Visible
        {
            get { return visible; }
            set { visible = value; Dirty = true; }
        }

        #region Event functions

        #region MouseEvents
        public virtual void OnMouseEnter() { }
        public virtual void OnMouseLeave() { }
        public virtual void OnMouseHover() { }
        public virtual void OnMouseDown(MouseButton button) { }
        public virtual void OnMouseClick(MouseButton button) { }
        public virtual void OnMouseUp(MouseButton button) { }
        #endregion

        #region KeyboardEvents
        public virtual void OnKeyPress(Key key) { }
        public virtual void OnKeyDown(Key key) { }
        public virtual void OnKeyUp(Key key) { }
        #endregion

        private bool mouseInsideLastFrame = false;

        #endregion

        HashSet<MouseButton> mouseDownButtonState = new HashSet<MouseButton>();

        /// <summary>
        /// The update function gets called on every frame update.
        /// </summary>
        /// <param name="delta">The delta time of the current frame.</param>
        public void Update(double delta) 
        {
            // Return if widget is not visible
            if (!Visible)
                return;

            Time.StartTimer("Widget.Update()", "Gui");

            // Is the mouse over this widget?
            bool inside = UserInput.Mouse.IsInsideRect(Position, Position + Size);

            if (inside)
            {
                if (!mouseInsideLastFrame)
                {
                    GuiEventManager.Current.RaiseEvent(GuiEvent.MouseEnter, this);
                }

                GuiEventManager.Current.RaiseEvent(GuiEvent.MouseHover, this);

                Widget.CurrentlyActive = this;
                GuiRoot.Root.MouseOverGuiElement = true;

                var thisframe = new HashSet<MouseButton>();

                foreach (var button in UserInput.Mouse.GetDownButtons())
                {
                    GuiEventManager.Current.RaiseEvent(GuiEvent.MouseDown, this, (object)button);
                    
                    // Hold all the button states on this frame
                    thisframe.Add(button);
                }

                foreach (var button in mouseDownButtonState)
                {
                    if (!thisframe.Contains(button)) GuiEventManager.Current.RaiseEvent(GuiEvent.MouseUp, this, (object)button);
                }

                mouseDownButtonState = thisframe;

                foreach (var button in UserInput.Mouse.GetClickedButtons())
                {
                    GuiEventManager.Current.RaiseEvent(GuiEvent.MouseClick, this, (object)button);
                }
            }
            else if(mouseInsideLastFrame)
            {
                GuiEventManager.Current.RaiseEvent(GuiEvent.MouseLeave, this);
            }

            OnUpdate(delta);

            mouseInsideLastFrame = inside;

            Time.StopTimer("Widget.Update()");
        }

        public virtual void OnUpdate(double delta) { }

        /// <summary>
        /// Returns all
        /// </summary>
        /// <returns></returns>
        public virtual void GetVertices(List<Vector2> vertices, List<Vector3> uv, List<Vector4> colors) {}

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
        public virtual string CustomRenderLayer
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// Gets the currently active widget. Used internally for calling user input event functions, but is exposed as public for convenience.
        /// </summary>
        public static Widget CurrentlyActive
        {
            get;
            internal set;
        }

    }
}
