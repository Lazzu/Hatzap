using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hatzap.Sprites
{
    class SpritePaintQueue
    {
        Dictionary<SpriteAtlas, SpritePaintBatch> batches = new Dictionary<SpriteAtlas,SpritePaintBatch>();
        List<SpriteAtlas> batchIndices = new List<SpriteAtlas>();

        public void Insert(SpriteRenderData data)
        {
            var key = data.Sprite.Atlas;

            SpritePaintBatch batch;
            if(!batches.TryGetValue(key, out batch))
            {
                batch = new SpritePaintBatch();
                batchIndices.Add(key);
                batches.Add(key, batch);
            }

            batch.Insert(data);
        }

        public IEnumerable<bool> GetVertices(List<SpriteVertex> vertices, List<int> indices)
        {
            int c = batchIndices.Count;
            for(int i = 0; i < c; c++)
            {
                var key = batchIndices[i];

                foreach(var tmp in batches[key].GetVertices(vertices, indices))
                {
                    yield return tmp;
                }
            }
        }
    }
}
