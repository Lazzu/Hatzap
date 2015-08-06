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
using OpenTK.Graphics.OpenGL;

namespace Hatzap.Rendering
{
    public class TextureBatch
    {
        public Dictionary<string, Texture> Textures { get; set; }
        public Dictionary<Mesh, InstancedBatch> Instanced;
        public Dictionary<Material, List<Renderable>> Batched = new Dictionary<Material,List<Renderable>>();
        public Dictionary<Material, int> BatchCount = new Dictionary<Material, int>();

        int count = 0;

        public RenderQueue RenderQueue { get; set; }

        Dictionary<string, int> locations = new Dictionary<string, int>();

        public void Insert(Renderable data)
        {
            if (data == null)
                return;

            if (Instanced == null && !Batched.ContainsKey(data.Material))
            {
                if (RenderQueue.AllowInstancing && GPUCapabilities.Instancing && data is Model)
                {
                    Instanced = new Dictionary<Mesh,InstancedBatch>();
                    Debug.WriteLine("Instantiated InstacedBatch");
                }
                else
                {
                    Batched.Add(data.Material, new List<Renderable>());
                    BatchCount.Add(data.Material, 0);
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
                var count = BatchCount[data.Material];
                if (Batched[data.Material].Count > count)
                {
                    Batched[data.Material][count] = data;
                }
                else
                {
                    Batched[data.Material].Add(data);
                }
                BatchCount[data.Material] = 1 + count;
            }

            Textures = data.Material.Textures;

            if(locations.Count == 0)
            {
                foreach (var texture in Textures)
                {
                    var uniform = texture.Key;
                    var location = GL.GetUniformLocation(data.Material.ShaderProgram.ID, uniform);
                    locations.Add(uniform, location);
                }
            }
            

            
        }

        internal int Render()
        {
            int triangles = 0;

            if(Textures != null || Textures.Count != 0)
            {
                int i = 0;
                foreach (var texture in Textures)
                {
                    var name = texture.Key;
                    var t = texture.Value;

                    GL.Uniform1(locations[name], i);
                    GL.ActiveTexture(TextureUnit.Texture0 + i);

                    t.Bind();
                    t.UpdateQuality();

                    i++;
                }
            }

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
                foreach (var BatchQueue in Batched)
                {
                    var batch = BatchQueue.Value;
                    var count = BatchCount[BatchQueue.Key];

                    for (int i = 0; i < count; i++)
                    {

                        // Take object from the batch queue
                        var obj = batch[i];

                        if (obj == null)
                            continue;

                        var model = obj as Model;

                        // Ugly, refactor this somehow. :(
                        if (model != null)
                        {
                            model.Mesh.VertexAttribLocation = model.Material.ShaderProgram.GetAttribLocation("vertex");
                            model.Mesh.NormalAttribLocation = model.Material.ShaderProgram.GetAttribLocation("normal");
                            model.Mesh.TangentAttribLocation = model.Material.ShaderProgram.GetAttribLocation("tangent");
                            model.Mesh.BinormalAttribLocation = model.Material.ShaderProgram.GetAttribLocation("binormal");
                            model.Mesh.UVAttribLocation = model.Material.ShaderProgram.GetAttribLocation("uv");
                            model.Mesh.ColorAttribLocation = model.Material.ShaderProgram.GetAttribLocation("color");
                        }

                        //model.Transform.CalculateMatrix();
                        //model.Material.ShaderProgram.SendUniform("mModel", ref model.Transform.Matrix);

                        // Put a null back
                        batch[i] = null;

                        // Call render
                        obj.Render();

                        triangles += obj.Triangles;
                    }

                    BatchCount[BatchQueue.Key] = 0;
                }
                
            }

            if (Textures != null || Textures.Count != 0)
            {
                foreach (var texture in Textures)
                {
                    var t = texture.Value;
                    t.UnBind();
                }
            }            

            return triangles;
        }
    }
}
