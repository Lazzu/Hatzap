using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hatzap.Textures
{
    public class Texture
    {
        public int ID { get; set; }

        public TextureTarget TextureTarget { get; protected set; }

        public Texture()
        {
            TextureTarget = TextureTarget.Texture2D;
        }

        public void Bind()
        {
            GL.BindTexture(TextureTarget, ID);
        }
    }
}
