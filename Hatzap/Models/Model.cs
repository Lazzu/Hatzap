using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hatzap.Rendering;
using Hatzap.Shaders;
using Hatzap.Textures;
using OpenTK;

namespace Hatzap.Models
{
    public class Model : Renderable
    {
        public override ShaderProgram Shader { get; set; }

        public override Texture Texture { get; set; }

        public override Material Material { get; set; }

        public Mesh Mesh { get; set; }

        public Model()
        {
            Material = new Material();
        }

        public override void Render()
        {
            if (Mesh == null)
                return;

            Shader.SendUniform("mModel", ref Transform.Matrix);

            foreach (var item in Material)
            {
                item.SendData(Shader);
            }

            Mesh.Draw();
        }

        public override int Triangles
        {
            get { return Mesh.Triangles; }
        }


    }
}
