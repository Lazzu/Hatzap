using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hatzap.Gui.Widgets
{
    public class StackContainer : WidgetGroup
    {
        public override void GetAnchorPointsForChild(Widget child, out float left, out float right, out float top, out float bottom)
        {
            left = LeftAnchorOffset;
            right = RightAnchorOffset;
            top = TopAnchorOffset;
            bottom = BottomAnchorOffset;

            int index = Widgets.IndexOf(child);

            if(index > -1)
            {
                top += index * top + GetChildrenTotalHeight(index);
            }
        }

        float GetChildrenTotalHeight(int index)
        {
            float height = 0;

            for (int i = 0; i < index; i++)
            {
                height += Widgets[i].size.Y;
            }

            return height;
        }
    }
}
