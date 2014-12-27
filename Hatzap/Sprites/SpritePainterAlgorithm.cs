using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hatzap.Models;
using Hatzap.Utilities;

namespace Hatzap.Sprites
{
    public class SpritePainterAlgorithm : ISpriteRenderQueueAlgorithm
    {
        SpritePaintQueue queue = new SpritePaintQueue();

        

        public void Enqueue(SpriteRenderData sprite)
        {
            
        }

        public void Render()
        {
            GLState.DepthTest = false;
            GLState.PrimitiveRestart = true;
            GLState.PrimitiveRestartIndex = 0xFFFF;

            List<SpriteVertex> vertices = new List<SpriteVertex>();
            List<int> indices = new List<int>();

            Queue<Mesh> meshes = new Queue<Mesh>();

            foreach (var item in queue.GetVertices(vertices, indices))
            {

                // Make a mesh out of vertices in list, and start a new list

            }

            GLState.DepthTest = true;
            GLState.PrimitiveRestart = false;
        }
    }
}
