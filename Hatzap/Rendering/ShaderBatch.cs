using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hatzap.Textures;

namespace Hatzap.Rendering
{
    public class ShaderBatch
    {
        public Dictionary<Texture, TextureBatch> TextureBatches = new Dictionary<Texture, TextureBatch>();

        public void Insert(RenderData data)
        {
            var texture = data.RenderObject.Texture;

            TextureBatch batch = null;

            if(!TextureBatches.TryGetValue(texture, out batch))
            {
                batch = new TextureBatch();
                TextureBatches.Add(texture, batch);
            }

            batch.Insert(data);
        }

        internal void Render()
        {
            foreach (var textureBatch in TextureBatches)
            {
                var texture = textureBatch.Key;
                var batchQueue = textureBatch.Value;

                texture.Bind();

                batchQueue.Render();

                texture.UnBind();
            }
        }
    }
}
