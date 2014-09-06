using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hatzap.Shaders
{
    public class ShaderException : GraphicsException
    {
        public ShaderException(string message) : base(message)
        {
        }

        public ShaderException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
