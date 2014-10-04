using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hatzap.Gui.Elements
{
    public class GuiElementCollection
    {
        public List<GuiElement> Elements { get; set; }

        public GuiElementCollection()
        {
            Elements = new List<GuiElement>();
        }
    }
}
