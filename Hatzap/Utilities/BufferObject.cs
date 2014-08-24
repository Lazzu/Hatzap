using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hatzap.Utilities
{
    public class BufferObject
    {
        public int ID { get; protected set; }

        public BufferTarget Target { get; protected set; }

        public BufferObject(BufferTarget target)
        {
            ID = GL.GenBuffer();
            Target = target;
        }

        public void Bind()
        {
            GL.BindBuffer(Target, ID);
        }
    }
}
