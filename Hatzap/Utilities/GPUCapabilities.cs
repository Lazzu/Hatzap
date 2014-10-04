using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace Hatzap.Utilities
{
    public static class GPUCapabilities
    {
        static bool initialized = false;

        static HashSet<string> extensions = new HashSet<string>();

        public static void Initialize()
        {
            if (initialized)
                return;

            initialized = true;

            var ext = GL.GetString(StringName.Extensions);

            var exts = ext.Split(' ');

            foreach (var item in exts)
	        {
                extensions.Add(item);
	        }
        }
        
        public static bool IsExtensionAvailable(string extension)
        {
            Initialize();
            return extensions.Contains(extension);
        }
    }
}
