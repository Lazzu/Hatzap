using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelExample
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using(ModelExampleWindow window = new ModelExampleWindow())
            {
                window.Run(60, 60);
            }
        }
    }
}
