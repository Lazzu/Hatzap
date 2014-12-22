using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Hatzap.Textures;
using Hatzap.Utilities;
using OpenTK.Graphics.OpenGL;

namespace Hatzap.Sprites
{
    [XmlRootAttribute("SpriteAtlas", IsNullable = false)]
    public class SpriteAtlas
    {
        [XmlIgnore]
        public Texture Texture { get; set; }

        [XmlElement("Texture")]
        public TextureMeta TextureMeta { get; set; }

        [XmlArrayAttribute("Sprites")]
        public List<Sprite> Sprites { get; protected set; }

        [XmlArrayAttribute("Quality")]
        public TextureQuality Quality { get; protected set; }

        public SpriteAtlas()
        {
            Sprites = new List<Sprite>();
        }

        void Initialize()
        {
            Texture = new Texture();
            Texture.Load(TextureMeta);
            Texture.Bind();
        }

        public static SpriteAtlas Load(string file)
        {
            var atlas = XML.Read.FromFile<SpriteAtlas>(file);

            if(atlas != null)
                atlas.Initialize();

            return atlas;
        }

        public static void Save(string file, SpriteAtlas atlas)
        {
            XML.Write.ToFile(atlas, file);
        }
    }
}
