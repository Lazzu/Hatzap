using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Hatzap.Sprites
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SpriteRenderData
    {
        public Vector3 Vertex;
        public Vector2 UV;

        /// <summary>
        /// The sprite position on screen in pixels
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// The sprite size on screen in pixels
        /// </summary>
        public Vector2 Size;

        /// <summary>
        /// The sprite rotation around z-axis in radians
        /// </summary>
        public float Rotation;

        

        public static int SizeInBytes
        {
            get { return Vector3.SizeInBytes * 2 + Vector2.SizeInBytes * 2 + sizeof(float); }
        }
    }
}
