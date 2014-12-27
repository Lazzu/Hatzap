using System;

namespace HelloTriangle
{
    class Program
    {
        static void Main(string[] args)
        {
            using (HelloTriangleWindow gw = new HelloTriangleWindow())
            {
                gw.VSync = OpenTK.VSyncMode.On;
                gw.Run();
            }

        }
    }
}
