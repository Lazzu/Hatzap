using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hatzap.Gui.Fonts
{
    [Serializable]
    public class FontCollection
    {
        public List<FontInfo> Fonts { get; set; }

        public FontCollection()
        {
            Fonts = new List<FontInfo>();
        }
    }
}
