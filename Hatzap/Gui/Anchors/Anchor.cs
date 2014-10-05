using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hatzap.Gui.Widgets;
using OpenTK;

namespace Hatzap.Gui.Anchors
{
    public class Anchor
    {
        public Dictionary<AnchorDirection, AnchorType> Directions { get; protected set; }

        public Widget Parent { get; internal set; }

        public Anchor()
        {
            Directions = new Dictionary<AnchorDirection, AnchorType>();

            Directions[AnchorDirection.Left] = AnchorType.None;
            Directions[AnchorDirection.Right] = AnchorType.None;
            Directions[AnchorDirection.Top] = AnchorType.None;
            Directions[AnchorDirection.Bottom] = AnchorType.None;
        }

        /// <summary>
        /// Calculates widget position coordinate constraints and sets the new coordinates to it's anchor.Parent widget.
        /// </summary>
        /// <param name="newPos">New desired position</param>
        /// <param name="newSize">New desired size</param>
        public void SetCoordinates(Vector2 newPos, Vector2 newSize)
        {
            if (Parent == null)
                return;

            bool hasparent = Parent.WidgetGroup != null;

            switch (Directions[AnchorDirection.Left])
            {
                case AnchorType.None:
                    Parent.position.X = newPos.X;
                    break;
                case AnchorType.Snap:
                    if (hasparent)
                    {
                        Parent.position.X = Parent.WidgetGroup.Position.X + Parent.WidgetGroup.LeftAnchorOffset;
                    }
                    break;
                case AnchorType.Relative:
                    throw new NotImplementedException("Relative anchoring not yet implemented.");
            }

            switch (Directions[AnchorDirection.Right])
            {
                case AnchorType.None:
                    Parent.size.X = newSize.X;
                    break;
                case AnchorType.Snap:
                    if (hasparent)
                    {
                        Parent.size.X = Parent.WidgetGroup.Position.X + Parent.WidgetGroup.Size.X + Parent.WidgetGroup.RightAnchorOffset - Parent.position.X;

                        Debug.WriteLine("Parent.WidgetGroup.size: " + Parent.WidgetGroup.size);

                    }
                    break;
                case AnchorType.Relative:
                    throw new NotImplementedException("Relative anchoring not yet implemented.");
            }

            switch (Directions[AnchorDirection.Top])
            {
                case AnchorType.None:
                    Parent.position.Y = newPos.Y;
                    break;
                case AnchorType.Snap:
                    if (hasparent)
                    {
                        Parent.position.Y = Parent.WidgetGroup.Position.Y + Parent.WidgetGroup.TopAnchorOffset;
                    }
                    break;
                case AnchorType.Relative:
                    throw new NotImplementedException("Relative anchoring not yet implemented.");
            }

            switch (Directions[AnchorDirection.Bottom])
            {
                case AnchorType.None:
                    Parent.size.Y = newSize.Y;
                    break;
                case AnchorType.Snap:
                    if (hasparent)
                    {
                        Parent.size.Y = Parent.WidgetGroup.Position.Y + Parent.WidgetGroup.Size.Y + Parent.WidgetGroup.BottomAnchorOffset - Parent.position.Y;
                    }
                    break;
                case AnchorType.Relative:
                    throw new NotImplementedException("Relative anchoring not yet implemented.");
            }

        }
    }
}
