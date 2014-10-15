using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HatzapTestApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            using(HatzapGameWindow gw = new HatzapGameWindow())
            {
                gw.VSync = OpenTK.VSyncMode.On;
                gw.Run();
            }

        }
    }
}
