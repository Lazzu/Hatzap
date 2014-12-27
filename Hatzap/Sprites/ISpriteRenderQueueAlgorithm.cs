using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hatzap.Sprites
{
    public interface ISpriteRenderQueueAlgorithm
    {
        void Enqueue(SpriteRenderData sprite);
        void Render();
    }
}
