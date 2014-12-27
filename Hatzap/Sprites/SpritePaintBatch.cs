using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hatzap.Utilities;

namespace Hatzap.Sprites
{
    class SpritePaintBatch
    {
        Queue<SpriteRenderData> sprites = new Queue<SpriteRenderData>();

        internal IEnumerable<bool> GetVertices(List<SpriteVertex> vertices, List<int> indices)
        {
            SpriteRenderData sprite;
            while ((sprite = sprites.Dequeue()) != null)
            {
                int startIndex = vertices.Count;
                var spriteVert = sprite.Vertices;
                vertices.AddRange(spriteVert);

                for (int i = 0; i < spriteVert.Length; i++)
                {
                    indices.Add(i + startIndex);
                }

                indices.Add(GLState.PrimitiveRestartIndex);

                if (vertices.Count > GLState.PrimitiveRestartIndex - 100)
                {
                    yield return true;
                }
            }
        }

        internal void Insert(SpriteRenderData data)
        {
            sprites.Enqueue(data);
        }
    }
}
