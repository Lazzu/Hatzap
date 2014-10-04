using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hatzap.Rendering;
using OpenTK;

namespace Hatzap.Models
{
    public class Model : IRenderQueueable
    {
        public Shaders.ShaderProgram Shader { get; set; }

        public Textures.Texture Texture { get; set; }

        public Mesh Mesh { get; set; }

        public void Render()
        {
            if (Mesh == null)
                return;
            
            Mesh.Draw();
        }

        public int Triangles
        {
            get { return Mesh.Triangles; }
        }
    }
}
