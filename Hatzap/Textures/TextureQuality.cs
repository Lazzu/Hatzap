using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace Hatzap.Textures
{
    public class TextureQuality
    {
        public TextureMinFilter TextureMinFilter { get; set; }
        public TextureMagFilter TextureMagFilter { get; set; }
        public float Anisotrophy { get; set; }

        public TextureQuality()
        {
            TextureMinFilter = TextureMinFilter.Nearest;
            TextureMagFilter = TextureMagFilter.Nearest;
            Anisotrophy = 0;
        }
    }
}
