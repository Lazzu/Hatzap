using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hatzap.Assets;
using Hatzap.Textures;
using Hatzap.Utilities;
using OpenTK.Graphics.OpenGL;

namespace Hatzap.Sprites
{
    public class SpriteAtlasManager : AssetManagerBase<SpriteAtlas>
    {
        TextureManager textureManager;

        protected override SpriteAtlas LoadAsset(System.IO.Stream stream)
        {
            if (textureManager == null)
                textureManager = new TextureManager();

            var atlas = XML.Read.FromStream<SpriteAtlas>(stream);

            atlas.Atlas = textureManager.Get(atlas.TextureName, true);

            return atlas;
        }

        protected override void SaveAsset(SpriteAtlas asset, System.IO.Stream stream)
        {
            XML.Write.ToStream(asset, stream);
        }

        public override void ReleaseAll()
        {
            foreach (var item in loadedAssets)
            {
                item.Value.Release();
            }
        }
    }
}
