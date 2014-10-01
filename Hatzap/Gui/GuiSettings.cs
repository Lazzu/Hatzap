using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Text;
using System.Threading.Tasks;
using Hatzap.Gui.Fonts;

namespace Hatzap.Gui
{
    [XmlRoot]
    public class GuiSettings
    {
        public string DefaultFont { get; set; }
        public int Version { get; set; }

        public List<Font> Fonts { get; set; }

        public GuiSettings()
        {
            DefaultFont = "Assets/Fonts/OpenSans-Regular.ttf_sdf";
            Version = CurrentVersion;
        }

        public static GuiSettings Current = new GuiSettings();
        public static int CurrentVersion = 0;
    }
}
