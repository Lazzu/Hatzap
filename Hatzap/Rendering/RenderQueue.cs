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
    public class RenderQueue
    {
        Dictionary<ShaderProgram, ShaderBatch> SolidShaderBatches = new Dictionary<ShaderProgram, ShaderBatch>();
        Dictionary<ShaderProgram, ShaderBatch> TransparentShaderBatches = new Dictionary<ShaderProgram, ShaderBatch>();

        bool allowInstancing = true;

        public bool AllowInstancing { get { return allowInstancing; } set { allowInstancing = value; } }

        public int TrianglesDrawn { get; set; }

        public int Count { get; set; }

        public RenderQueue()
        {
            allowInstancing = GPUCapabilities.Instancing;
        }

        public void Insert(Renderable data)
        {
            Time.StartTimer("RenderQueue.Insert()", "Render");

            if (data.Shader == null) throw new ArgumentNullException("data.Shader", "Renderable's shader must not be null!");

            var shader = data.Shader;

            // Check if the object needs to be added in solid or transparent batch
            var dict = data.Material.Transparent ? TransparentShaderBatches : SolidShaderBatches;

            ShaderBatch batch = null;

            if (!dict.TryGetValue(shader, out batch))
            {
                batch = new ShaderBatch();
                batch.RenderQueue = this;
                dict.Add(shader, batch);
            }

            batch.Insert(data);
            Count++;

            Time.StopTimer("RenderQueue.Insert()");
        }

        public void Render()
        {
            Time.StartTimer("RenderQueue.Render()", "Render");

            TrianglesDrawn = 0;

            GLState.AlphaBleding = false;
            GLState.DepthTest = true;

            foreach (var shaderBatch in SolidShaderBatches)
            {
                TrianglesDrawn += shaderBatch.Value.Render();
            }
            
            GLState.AlphaBleding = true;

            foreach (var shaderBatch in TransparentShaderBatches)
            {
                TrianglesDrawn += shaderBatch.Value.Render();
            }
            
            Count = 0;

            Time.StopTimer("RenderQueue.Render()");
        }

    }
}
