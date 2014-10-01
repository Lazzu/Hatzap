using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Input;

namespace Hatzap.Gui.Widgets
{
    public abstract class Widget
    {
        public Vector2 Position;
        public Vector2 Size;

        public GuiTextureRegion[] TextureRegion;
        
        // Should only be accessed by property
        private int z;
        private bool dirty;

        public int Z { get { return z; } set { z = value; if (WidgetGroup != null) WidgetGroup.Sort(); } }
        public WidgetGroup WidgetGroup { get; set; }

        /// <summary>
        /// If set to true, this widget requires updating
        /// </summary>
        public bool Dirty { get { return dirty; } set { dirty = value; if (dirty) WidgetGroup.Dirty = dirty; } }

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

        public virtual void Update(double delta) { }
        public virtual GuiVertex[] GetVertices() { return null; }
    }
}
