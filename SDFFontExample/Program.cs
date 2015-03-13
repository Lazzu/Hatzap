using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDFFontExample
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using(var game = new SDFFontGameWindow())
            {
                game.Run();
            }
        }
    }
}
