using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hatzap.Gui.Widgets;

namespace Hatzap.Gui
{
    [Serializable]
    public class ElementCollection
    {
        public List<WidgetInfo> Elements { get; set; }

        public ElementCollection()
        {
            Elements = new List<WidgetInfo>();
        }

        public WidgetInfo GetInfo<T>(T widget) where T : Widget
        {
            var type = widget.GetType().ToString();

            foreach (var item in Elements)
            {
                if (item.WidgetType == type)
                    return item;
            }

            return null;
        }
    }
}
