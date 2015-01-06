using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hatzap.Sprites
{
    public class SpriteBatch
    {
        List<SpriteRenderData> queue = new List<SpriteRenderData>();

        SpriteAtlas current = null;

        public void BeginBatch(SpriteAtlas atlas)
        {
            current = atlas;
        }

        public void EndBatch()
        {
            current = null;
        }
    }
}
