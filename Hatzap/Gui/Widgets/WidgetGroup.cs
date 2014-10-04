using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hatzap.Gui.Widgets
{
    public class WidgetGroup : Widget
    {
        List<Widget> widgets = new List<Widget>();

        public List<Widget> Widgets { get { return widgets; } }

        public void SortChildWidgets()
        {
            // TODO: Replace bubble sort with something more efficient if needed.
            for (int iterator = 0; iterator < widgets.Count; iterator++)
            {
                bool swap = false;
                for (int index = 0; index < widgets.Count - 1; index++)
                {
                    if (widgets[index].Z > widgets[index + 1].Z)
                    {
                        var tmp = widgets[index];
                        widgets[index] = widgets[index + 1];
                        widgets[index + 1] = tmp;
                        swap = true;
                    }
                }
                if (!swap) // No more swapping was needed, break the loop
                    break;
            }
        }

        public void AddChildWidget(Widget item)
        {
            widgets.Add(item);
            item.WidgetGroup = this;
            SortChildWidgets();
            Dirty = true;
        }

        public void ClearChildWidgets()
        {
            foreach (var item in widgets)
            {
                item.WidgetGroup = null;
            }
            widgets.Clear();
            Dirty = true;
        }

        public int ChildWidgetCount
        {
            get { return widgets.Count; }
        }

        public bool RemoveChildWidget(Widget item)
        {
            if (widgets.Remove(item))
            {
                item.WidgetGroup = null;
                Dirty = true;
                return true;
            }

            return false;
        }
    }
}
