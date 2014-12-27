using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace Hatzap.Sprites
{
    public class SpriteRenderData
    {
        public Sprite Sprite { get; set; }
        public Vector4 Color { get; set; }
        public Vector3 Position { get; set; }
        public Vector2 Size { get; set; }

        SpriteVertex[] vertices;

        public SpriteVertex[] Vertices
        {
            get
            {
                if(vertices == null)
                {
                    GenerateVertices();
                }

                for (int i = 0; i < vertices.Length; i++ )
                {
                    vertices[i].position = Position;
                    vertices[i].scale = Size;
                    vertices[i].color = Color;
                }

                return vertices;
            }
        }

        private void GenerateVertices()
        {
            int c = Sprite.Vertices.Length;
            vertices = new SpriteVertex[c];

            Array.Copy(Sprite.Vertices, vertices, c);
        }
    }
}
