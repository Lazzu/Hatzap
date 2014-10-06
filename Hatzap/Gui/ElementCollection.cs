using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
