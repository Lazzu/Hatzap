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
        public Dictionary<Texture, TextureBatch> TextureBatches = new Dictionary<Texture, TextureBatch>();

        // Statics to improve performance a little bit
        static Texture texture;
        static TextureBatch batchQueue;

        public void Insert(Renderable data)
        {
            Program = data.Shader;
            texture = data.Texture;

            if(texture == null)
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
                if (!TextureBatches.TryGetValue(texture, out batchQueue))
                {
                    batchQueue = new TextureBatch();
                    batchQueue.RenderQueue = RenderQueue;
                    TextureBatches.Add(texture, batchQueue);
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
