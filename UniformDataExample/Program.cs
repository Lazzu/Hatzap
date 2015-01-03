using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniformDataExample
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using(UniformDataExampleWindow window = new UniformDataExampleWindow())
            {
                window.Run(60, 60);
            }
        }
    }
}
