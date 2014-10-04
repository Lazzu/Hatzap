using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Hatzap.Rendering
{
    public class TextureBatch
    {
        public List<RenderData> BatchQueue = new List<RenderData>();

        int count = 0;

        internal void Insert(RenderData data)
        {
            if(BatchQueue.Count > count)
            {
                BatchQueue[count] = data;
            }
            else
            {
                BatchQueue.Add(data);
            }
            count++;
        }

        internal int Render()
        {
            int triangles = 0;

            for(int i = 0; i < count; i++)
            {
                // Take object from the batch queue
                var obj = BatchQueue[i];

                // Put a null back
                BatchQueue[i] = null;

                // Send uniforms to the shader
                var shader = obj.RenderObject.Shader;
                if(obj.UniformData != null && obj.UniformData.Count > 0)
                {
                    for (int u = 0; u < obj.UniformData.Count; u++ )
                    {
                        obj.UniformData[u].SendData(shader);
                    }
                }
                
                // Call render
                obj.RenderObject.Render();

                triangles += obj.RenderObject.Triangles;

                // Release render data back to pool
                RenderDataPool.Release(obj);
            }
            count = 0;

            return triangles;
        }
    }
}
