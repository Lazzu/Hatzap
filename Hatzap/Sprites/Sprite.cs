using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hatzap.Textures;
using OpenTK;

namespace Hatzap.Sprites
{
    public class Sprite
    {
        public string Name { get; set; }

        public SpriteAtlas Atlas { get; set; }

        public Vector2 Size { get; set; }

        public Vector3[] Vertices { get; set; }

        public Vector2[] TextureCoordinates { get; set; }
    }
}
