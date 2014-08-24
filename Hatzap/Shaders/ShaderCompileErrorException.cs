using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hatzap.Shaders
{
    public class ShaderCompileErrorException : Exception
    {
        public ShaderCompileErrorException(string message)
            : base(message)
        {
        }

        public ShaderCompileErrorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
