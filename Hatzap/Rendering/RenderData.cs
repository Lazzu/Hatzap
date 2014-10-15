using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Hatzap.Rendering
{
    public class RenderData
    {
        public IRenderable RenderObject { get; set; }
        public List<IUniformData> UniformData { get; set; }

    }
}
