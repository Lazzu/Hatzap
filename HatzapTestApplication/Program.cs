using System;

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
