using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultitexturingExample
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (MultitexturingExampleWindow window = new MultitexturingExampleWindow())
            {
                window.Run(60, 60);
            }
        }
    }
}
