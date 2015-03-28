using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Hatzap.Textures;
using OpenTK;

namespace Hatzap.Sprites
{
    public class Sprite
    {
        [XmlAttribute("ID")]
        public string Name { get; set; }

        public SpriteAtlas Atlas { get; set; }

        public Vector2 Size { get; set; }

        [XmlArrayItem(ElementName = "Pos")]
        public Vector3[] Vertices { get; set; }

        [XmlArrayItem(ElementName = "UV")]
        public Vector2[] TextureCoordinates { get; set; }

        [XmlArrayItem(ElementName="Index")]
        public uint[] Indices { get; set; }
    }
}

