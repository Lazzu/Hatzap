using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hatzap.Shaders;
using OpenTK;

namespace Hatzap.Rendering
{
    public static class RenderQueue
    {
        static Dictionary<ShaderProgram, ShaderBatch> ShaderBatches = new Dictionary<ShaderProgram, ShaderBatch>();

        public static int TrianglesDrawn { get; set; }

        public static int Count { get; set; }

        public static void Insert(RenderData data)
        {
            var shader = data.RenderObject.Shader;

            ShaderBatch batch = null;

            if (!ShaderBatches.TryGetValue(shader, out batch))
            {
                batch = new ShaderBatch();
                ShaderBatches.Add(shader, batch);
            }

            batch.Insert(data);
            Count++;

        }

        public static void Render()
        {
            TrianglesDrawn = 0;

            foreach (var shaderBatch in ShaderBatches)
            {
                var shader = shaderBatch.Key;
                var textureBatch = shaderBatch.Value;

                shader.Enable();

                TrianglesDrawn += textureBatch.Render();

                shader.Disable();
            }
            Count = 0;
        }
    }
}
