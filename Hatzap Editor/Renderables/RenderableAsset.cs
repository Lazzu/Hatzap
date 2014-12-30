using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hatzap.Rendering;

namespace Hatzap_Editor.Renderables
{
    abstract class RenderableAsset
    {
        public abstract Renderable Renderable
        {
            get;
        }
    }
}
