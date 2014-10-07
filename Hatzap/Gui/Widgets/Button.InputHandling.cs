using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Input;

namespace Hatzap.Gui.Widgets
{
	public delegate void OnMouseClick(MouseButton button);
    public delegate void OnMouseDown(MouseButton button);
    public delegate void OnMouseUp(MouseButton button);
    public delegate void OnMouseDoubleClick(MouseButton button);
    public delegate void OnMouseEnter();
    public delegate void OnMouseHover();
    public delegate void OnMouseLeave();
    

    public partial class Button
    {
        public OnMouseClick OnClick;
        public OnMouseDown OnDown;
        public OnMouseUp OnUp;
        public OnMouseDoubleClick OnDoubleClick;
        public OnMouseEnter OnEnter;
        public OnMouseHover OnHover;
        public OnMouseLeave OnLeave;

        public override void OnMouseClick(MouseButton button)
        {
            if (OnClick != null) OnClick(button);
        }

        public override void OnMouseEnter()
        {
            if (OnEnter != null) OnEnter();
        }

        public override void OnMouseHover()
        {
            if (OnHover != null) OnHover();   
        }

        public override void OnMouseLeave()
        {
            if (OnLeave != null) OnLeave();
        }

        public override void OnMouseDown(MouseButton button)
        {
            if (OnDown != null) OnDown(button);
        }

        public override void OnMouseUp(MouseButton button)
        {
            if (OnUp != null) OnUp(button);
        }
    }
}
