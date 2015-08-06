using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Hatzap.Shaders;
using Hatzap.Textures;

namespace Hatzap.Rendering
{
    public class ShaderBatch
    {
        public ShaderProgram Program { get; set; }
        public RenderQueue RenderQueue { get; set; }

        TextureBatch textureless;
        public Dictionary<Dictionary<string, Texture>, TextureBatch> TextureBatches = new Dictionary<Dictionary<string, Texture>, TextureBatch>();
        
        public void Insert(Renderable data)
        {
            Program = data.Material.ShaderProgram;
            var textures = data.Material.Textures;

            TextureBatch batchQueue;

            if (textures == null || textures.Count == 0)
            {
                if(textureless == null)
                {
                    textureless = new TextureBatch();
                    textureless.RenderQueue = RenderQueue;
                }

                batchQueue = textureless;
            }
            else
            {
                if (!TextureBatches.TryGetValue(textures, out batchQueue))
                {
                    batchQueue = new TextureBatch();
                    batchQueue.RenderQueue = RenderQueue;
                    TextureBatches.Add(textures, batchQueue);
                }
            }

            batchQueue.Insert(data);
        }

        internal int Render()
        {
            int triangles = 0;

            Program.Enable();
            Program.SendUniform("mViewProjection", ref Camera.Current.VPMatrix);
            Program.SendUniform("mNormal", ref Camera.Current.NormalMatrix);
            Program.SendUniform("EyeDirection", ref Camera.Current.Direction);

            if(textureless != null)
            {
                triangles += textureless.Render();
            }

            foreach (var textureBatch in TextureBatches)
            {
                triangles += textureBatch.Value.Render();
            }

            return triangles;
        }
    }
}
