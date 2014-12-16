using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Hatzap.Models;
using Hatzap.Textures;
using Hatzap.Utilities;
using OpenTK;

namespace Hatzap.Rendering
{
    public class TextureBatch
    {
        public Texture Texture { get; set; }
        public List<Renderable> BatchQueue;
        public Dictionary<Mesh, InstancedBatch> Instanced;

        int count = 0;

        public RenderQueue RenderQueue { get; set; }

        public void Insert(Renderable data)
        {
            

            if(Instanced == null && BatchQueue == null)
            {
                if (RenderQueue.AllowInstancing && GPUCapabilities.Instancing && data is Model)
                {
                    Instanced = new Dictionary<Mesh,InstancedBatch>();
                    Debug.WriteLine("Instantiated InstacedBatch");
                }
                else
                {
                    BatchQueue = new List<Renderable>();
                    Debug.WriteLine("Instantiated BatchQueue");
                }
            }
            
            if(Instanced != null)
            {
                var model = data as Model;
                var mesh = model.Mesh;

                InstancedBatch batch;
                if(!Instanced.TryGetValue(mesh, out batch))
                {
                    batch = new InstancedBatch(model);
                    Instanced.Add(mesh, batch);
                }

                batch.Insert(model);
            }
            else
            {
                if (BatchQueue.Count > count)
                {
                    BatchQueue[count] = data;
                }
                else
                {
                    BatchQueue.Add(data);
                }
                count++;
            }

            Texture = data.Texture;
        }

        internal int Render()
        {
            int triangles = 0;

            Texture.Bind();

            if(Instanced != null)
            {
                // Draw each instanced mesh
                foreach (var item in Instanced)
                {
                    triangles += item.Value.Draw();
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    // Take object from the batch queue
                    var obj = BatchQueue[i];

                    var model = obj as Model;

                    if (model != null)
                    {
                        model.Mesh.VertexAttribLocation = model.Shader.GetAttribLocation("vertex");
                        model.Mesh.NormalAttribLocation = model.Shader.GetAttribLocation("normal");
                        model.Mesh.TangentAttribLocation = model.Shader.GetAttribLocation("tangent");
                        model.Mesh.BinormalAttribLocation = model.Shader.GetAttribLocation("binormal");
                        model.Mesh.UVAttribLocation = model.Shader.GetAttribLocation("uv");
                        model.Mesh.ColorAttribLocation = model.Shader.GetAttribLocation("color");
                    }

                    // Put a null back
                    BatchQueue[i] = null;

                    // Call render
                    obj.Render();

                    triangles += obj.Triangles;
                }
                count = 0;
            }
            
            return triangles;
        }
    }
}
