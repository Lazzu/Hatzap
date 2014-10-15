using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hatzap.Shaders;
using Hatzap.Textures;

namespace Hatzap.Rendering
{
    public class ShaderBatch
    {
        public ShaderProgram Program { get; set; }

        public Dictionary<Texture, TextureBatch> TextureBatches = new Dictionary<Texture, TextureBatch>();
        
        public void Insert(Renderable data)
        {
            Program = data.Shader;

            var texture = data.Texture;

            TextureBatch batch = null;

            if(!TextureBatches.TryGetValue(texture, out batch))
            {
                batch = new TextureBatch();
                TextureBatches.Add(texture, batch);
            }

            batch.Insert(data);
        }

        internal int Render()
        {
            int triangles = 0;

            Program.Enable();
            Program.SendUniform("mViewProjection", ref Camera.Current.VPMatrix);

            foreach (var textureBatch in TextureBatches)
            {
                var texture = textureBatch.Key;
                var batchQueue = textureBatch.Value;

                texture.Bind();

                triangles += batchQueue.Render();

                texture.UnBind();
            }

            return triangles;
        }
    }
}
