using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Hatzap.Gui.Fonts
{
    public class FontInfo
    {
        [XmlAttribute]
        public string FontFamily { get; set; }
        [XmlAttribute]
        public string FontDataFile { get; set; }
        [XmlAttribute]
        public string FontTextureFile { get; set; }
    }
}
