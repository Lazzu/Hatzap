using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubemapExample
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (CubemapExampleWindow window = new CubemapExampleWindow())
            {
                window.Run(60, 60);
            }
        }
    }
}
