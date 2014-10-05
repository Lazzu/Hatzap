using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hatzap.Gui.Widgets;
using OpenTK;

namespace Hatzap.Gui.Anchors
{
    public class Anchor
    {
        public Dictionary<AnchorDirection, AnchorType> Directions { get; set; }

        public Widget Parent { get; set; }

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
        public void SetCoordinates(ref Vector2 newPos, ref Vector2 newSize)
        {
            var oldMin = Parent.Position;
            var oldMax = Parent.Position + Parent.Size;

            var newMin = newPos;
            var newMax = newPos + newSize;

            Vector2 calculatedMin = oldMin;
            Vector2 calculatedMax = oldMax;

            var left = Directions[AnchorDirection.Left];
            var right = Directions[AnchorDirection.Right];
            var top = Directions[AnchorDirection.Top];
            var bottom = Directions[AnchorDirection.Bottom];

            if (left == AnchorType.Relative || right == AnchorType.Relative || top == AnchorType.Relative || bottom == AnchorType.Relative)
                throw new NotImplementedException("Relative anchoring not yet implemented.");
            
            bool hasparent = Parent.WidgetGroup != null;

            switch (left)
            {
                case AnchorType.None:
                    calculatedMin.X = newMin.X;
                    break;
                case AnchorType.Snap:
                    if (hasparent)
                    {
                        calculatedMin.X = Parent.WidgetGroup.Position.X + Parent.WidgetGroup.LeftAnchorOffset;
                    }
                    break;
            }

            switch (right)
            {
                case AnchorType.None:
                    calculatedMax.X = newMax.X;
                    break;
                case AnchorType.Snap:
                    if (hasparent)
                    {
                        calculatedMax.X = Parent.WidgetGroup.Position.X + Parent.WidgetGroup.Size.X + Parent.WidgetGroup.RightAnchorOffset;
                    }
                    break;
            }

            switch (top)
            {
                case AnchorType.None:
                    calculatedMin.Y = newMin.Y;
                    break;
                case AnchorType.Snap:
                    if (hasparent)
                    {
                        calculatedMin.Y = Parent.WidgetGroup.Position.Y + Parent.WidgetGroup.TopAnchorOffset;
                    }
                    break;
            }

            switch (bottom)
            {
                case AnchorType.None:
                    calculatedMin.Y = newMin.Y;
                    break;
                case AnchorType.Snap:
                    if (hasparent)
                    {
                        calculatedMin.Y = Parent.WidgetGroup.Position.Y + Parent.WidgetGroup.Size.Y + Parent.WidgetGroup.BottomAnchorOffset;
                    }
                    break;
            }

            Parent.position = calculatedMin;
            Parent.size = calculatedMax - calculatedMin;
        }
    }
}
