using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hatzap.Textures;

namespace Hatzap.Sprites
{
    public class SpriteCollection
    {
        public TextureMeta Texture { get; set; }

        public List<Sprite> Sprites { get; protected set; }
    }
}
