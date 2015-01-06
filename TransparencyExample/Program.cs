using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransparencyExample
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using(TransparencyExampleWindow window = new TransparencyExampleWindow())
            {
                window.Run(60, 60);
            }
        }
    }
}
