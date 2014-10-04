using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hatzap.Shaders;
using Hatzap.Textures;
using OpenTK;

namespace Hatzap.Rendering
{
    public interface IRenderQueueable
    {
        ShaderProgram Shader { get; }
        Texture Texture{ get; }

        void Render();
    }
}
