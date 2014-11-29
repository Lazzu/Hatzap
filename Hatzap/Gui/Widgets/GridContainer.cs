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

        public List<int> RowHeights { get; protected set; }
        public List<int> CellWidths { get; protected set; }

        public GridContainer()
        {
            Columns = 1;
            Rows = 1;
            RowHeights = new List<int>();
            CellWidths = new List<int>();
        }

        public override void GetAnchorPointsForChild(Widget child, out float left, out float right, out float top, out float bottom)
        {
            left = 0;
            right = 0;
            top = 0;
            bottom = 0;

            int index = Widgets.IndexOf(child);

            if(index > -1)
            {
                int x = index % Columns;
                int y = (index - x) / Columns;

                for(int i = 0; i < x; i++)
                {
                    left += CellWidths[i] + LeftAnchorOffset;
                }

                for (int i = 0; i < y; i++)
                {
                    top += RowHeights[i] + TopAnchorOffset;
                }

                int cellWidth = CellWidths[x];
                int cellHeight = RowHeights[y];

                right = left + cellWidth;
                bottom = top + cellHeight;

                left += LeftAnchorOffset;
                right += RightAnchorOffset;
                top += TopAnchorOffset;
                bottom += BottomAnchorOffset;
            }

            
        }
    }
}
