using System;

namespace TextureExample
{
    class Program
    {
        static void Main(string[] args)
        {
            using(TextureExampleWindow gw = new TextureExampleWindow())
            {
                gw.VSync = OpenTK.VSyncMode.On;
                gw.Run();
            }

        }
    }
}

