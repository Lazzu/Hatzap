using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Hatzap.Gui.Fonts
{
    [StructLayout(LayoutKind.Sequential)]
    public struct GPUGlyph
    {
        public Vector2 Vertex;
        public Vector2 TCoord;
        public Vector4 Color;
        public Vector4 BorderColor;   

        public static int SizeInBytes
        {
            get
            {
                return Vector2.SizeInBytes +
                    Vector2.SizeInBytes +
                    Vector4.SizeInBytes +
                    Vector4.SizeInBytes;
            }
        }
    }
}
