using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Hatzap.Gui
{
    public class WidgetInfo
    {
        [XmlAttribute("Type")]
        public string WidgetType { get; set; }

        [XmlArrayItem(ElementName="Slice")]
        public List<GuiTextureRegion> Slices { get; set; }

        public WidgetInfo()
        {
            Slices = new List<GuiTextureRegion>();
        }
    }
}
