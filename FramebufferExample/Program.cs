using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FramebufferExample
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using(FramebufferExampleWindow window = new FramebufferExampleWindow())
            {
                window.Run();
            }
        }
    }
}
