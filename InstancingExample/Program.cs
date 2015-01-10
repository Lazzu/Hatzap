using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstancingExample
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using(InstancingExampleWindow window = new InstancingExampleWindow())
            {
                window.Run(60, 60);
            }
        }
    }
}
