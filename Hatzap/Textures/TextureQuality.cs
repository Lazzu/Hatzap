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
        public TextureWrapMode TextureWrapMode_S { get; set; }
        public TextureWrapMode TextureWrapMode_T { get; set; }

        protected bool isDirty = true;

        public TextureQuality()
        {
            TextureMinFilter = TextureMinFilter.Nearest;
            TextureMagFilter = TextureMagFilter.Nearest;
            TextureWrapMode_S = TextureWrapMode.Repeat;
            TextureWrapMode_T = TextureWrapMode.Repeat;
            Anisotrophy = 0;
        }

        /// <summary>
        /// TODO: Move dirty check here and update only the changed parts
        /// </summary>
        public void Update()
        {

        }
    }
}
