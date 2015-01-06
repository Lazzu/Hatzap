using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpriteExample
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using(SpriteExampleWindow window = new SpriteExampleWindow())
            {
                window.Run(60,60);
            }
        }
    }
}
