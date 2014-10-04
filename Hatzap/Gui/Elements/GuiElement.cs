using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hatzap.Gui.Elements
{
    public class GuiElement
    {
        public List<GuiTextureRegion> Regions { get; set; }

        public GuiElement()
        {
            Regions = new List<GuiTextureRegion>();
        }
    }
}
