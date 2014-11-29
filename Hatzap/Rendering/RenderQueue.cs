using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hatzap.Shaders;
using Hatzap.Utilities;
using OpenTK;

namespace Hatzap.Rendering
{
    public static class RenderQueue
    {
        static Dictionary<ShaderProgram, ShaderBatch> ShaderBatches = new Dictionary<ShaderProgram, ShaderBatch>();

        public static int TrianglesDrawn { get; set; }

        public static int Count { get; set; }

        public static void Insert(Renderable data)
        {
            Time.StartTimer("RenderQueue.Insert()", "Render");

            if (data.Shader == null) throw new ArgumentNullException("data.Shader", "Renderable's shader must not be null!");
            if (data.Texture == null) throw new ArgumentNullException("data.Texture", "Renderable's texture must not be null!");

            var shader = data.Shader;

            ShaderBatch batch = null;

            if (!ShaderBatches.TryGetValue(shader, out batch))
            {
                batch = new ShaderBatch();
                ShaderBatches.Add(shader, batch);
            }

            batch.Insert(data);
            Count++;

            Time.StopTimer("RenderQueue.Insert()");
        }

        public static void Render()
        {
            Time.StartTimer("RenderQueue.Render()", "Render");

            TrianglesDrawn = 0;

            foreach (var shaderBatch in ShaderBatches)
            {
                var shader = shaderBatch.Key;
                var textureBatch = shaderBatch.Value;

                TrianglesDrawn += textureBatch.Render();
            }
            Count = 0;

            Time.StopTimer("RenderQueue.Render()");
        }

        static bool allowInstancing = true;
        public static bool AllowInstancing { get { return allowInstancing; } set { allowInstancing = value; } }
    }
}
