using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hatzap.Textures;

namespace Hatzap.Gui.Fonts
{
    public static class FontManager
    {
        static Dictionary<string, Font> fonts = new Dictionary<string, Font>();

        public static Font Get(string font)
        {
            Font fnt = null;
            fonts.TryGetValue(font, out fnt);
            return fnt;
        }
    }
}
