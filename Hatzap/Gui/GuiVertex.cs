using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Hatzap.Gui
{
    public struct GuiVertex
    {
        public Vector2 Position;
        public Vector2 TextureCoordinates;
        public int TexturePage;

        public static int SizeInBytes
        {
            get
            {
                return Vector2.SizeInBytes * 2
                    + sizeof(int);
            }
        }
    }
}
