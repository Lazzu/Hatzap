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

        public void Sort()
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
                if (!swap)
                    break;
            }
        }

        public void Add(Widget item)
        {
            widgets.Add(item);
            item.WidgetGroup = this;
            Sort();
            Dirty = true;
        }

        public void Clear()
        {
            foreach (var item in widgets)
            {
                item.WidgetGroup = null;
            }
            widgets.Clear();
            Dirty = true;
        }

        public int Count
        {
            get { return widgets.Count; }
        }

        public bool Remove(Widget item)
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
