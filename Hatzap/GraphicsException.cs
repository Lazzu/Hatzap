using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hatzap
{
    public class GraphicsException : Exception
    {
        public GraphicsException(string message) : base(message)
        {
        }

        public GraphicsException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
