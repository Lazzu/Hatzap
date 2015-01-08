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
        public Texture Atlas { get; set; }

        public List<Sprite> Sprites { get; set; }

        public bool PremultipliedAlpha { get; set; }

        /// <summary>
        /// Get a sprite by it's name. This is slow operation, use only on loading and cache the results.
        /// </summary>
        /// <param name="key">Sprite name</param>
        /// <returns>A sprite if found by it's name. Null otherwise.</returns>
        public Sprite this[string key]
        {
            get
            {
                if (Sprites == null)
                    return null;

                for(int i = 0; i < Sprites.Count; i++)
                {
                    if (Sprites[i].Name == key)
                        return Sprites[i];
                }

                return null;
            }
        }

        public void Release()
        {
            if(Atlas != null)
            {
                Atlas.Release();
            }
            Atlas = null;
        }
        ~SpriteAtlas()
        {
            if (Atlas != null)
                throw new Exception("SpriteAtlas leaked! Release unneeded atlases with SpriteAtlas.Release()");
        }
    }
}
