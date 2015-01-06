using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Hatzap.Textures;

namespace Hatzap.Sprites
{
    public class SpriteAtlas
    {
        public string TextureName { get; set; }

        [XmlIgnore]
        Texture atlas = null;

        [XmlIgnore]
        public Texture Atlas { 
            get
            {
                if (atlas == null)
                    Prepare();

                return atlas;
            }
        }

        public List<Sprite> Sprites { get; set; }

        public void Prepare()
        {
            if (TextureName == string.Empty)
                return;

            

            /*atlas = new Texture();
            atlas.Load(meta);*/
        }
    }
}
