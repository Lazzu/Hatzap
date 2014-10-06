using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hatzap.Gui.Widgets
{
    public class GridContainer : WidgetGroup
    {
        public int Columns { get; set; }
        public int Rows { get; set; }

        public GridContainer()
        {
            Columns = 1;
            Rows = 1;
        }

        public override void GetAnchorPointsForChild(Widget child, out float left, out float right, out float top, out float bottom)
        {
            left = LeftAnchorOffset;
            right = RightAnchorOffset;
            top = TopAnchorOffset;
            bottom = BottomAnchorOffset;

            int index = Widgets.IndexOf(child);

            if(index > -1)
            {
                int x = index % Columns;
                int y = (index - x) / Columns;

                int cellWidth = (int)(Size.X / Columns);
                int cellHeight = (int)(Size.Y / Rows);

                left += x * cellWidth + x * left;
                top += y * cellHeight + y * top;

                right += (Columns - x - 1) * -cellWidth;
                bottom += (Rows - y - 1) * -cellHeight;
            }
        }
    }
}
