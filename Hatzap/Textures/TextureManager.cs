using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hatzap.Textures
{
    public static class TextureManager
    {
        static Dictionary<string, Texture> textures = new Dictionary<string, Texture>();

        public static Texture Get(string name)
        {
            Texture t = null;
            textures.TryGetValue(name, out t);
            return t;
        }
    }
}
