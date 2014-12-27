using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using OpenTK;

namespace Hatzap.Sprites
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SpriteVertex
    {
        public Vector2 vertex;
        public Vector2 scale;
        public Vector3 position;
        public Vector3 uv;
        public Vector4 color;

        public static int SizeInBytes
        {
            get { return Vector2.SizeInBytes * 2 + Vector3.SizeInBytes * 2 + Vector4.SizeInBytes; }
        }
    }
}
