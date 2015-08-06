using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Hatzap.Rendering;
using Hatzap.Shaders;
using Hatzap.Textures;
using Hatzap.Utilities;
using OpenTK;

namespace Hatzap.Models
{
    public class Model : Renderable
    {
        public override Material Material { get; set; }

        public Mesh Mesh { get; set; }

        public Model()
        {
            Material = new Material();
            Visible = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Render()
        {
            if (Mesh == null)
                return;

            Time.StartTimer("Model.Render()", "Rendering");

            Material.ShaderProgram.SendUniform("mModel", ref Transform.Matrix);

            foreach (var item in Material.Uniforms)
            {
                item.SendData(Material.ShaderProgram);
            }

            Mesh.Draw();

            Time.StopTimer("Model.Render()");
        }

        public override int Triangles
        {
            get { return Mesh.Triangles; }
        }


    }
}
