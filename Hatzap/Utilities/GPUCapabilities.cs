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

        public static string Version { get; private set; }
        public static string GLSL { get; private set; }

        public static void Initialize()
        {
            if (initialized)
                return;

            initialized = true;

            Version = GL.GetString(StringName.Version);
            GLSL = GL.GetString(StringName.ShadingLanguageVersion);

            int NumberOfExtensions;

            GL.GetInteger(GetPName.NumExtensions, out NumberOfExtensions);

            for(int i = 0; i < NumberOfExtensions; i++)
            {
                var ext = GL.GetString(StringNameIndexed.Extensions, i);
                extensions.Add(ext);
            }

            /*var exts = ext.Split(' ');

            foreach (var item in exts)
	        {
                extensions.Add(item);
	        }*/

            TextureCompression = IsExtensionAvailable("GL_EXT_texture_compression_s3tc");
            Instancing = IsExtensionAvailable("GL_ARB_instanced_arrays");

            int n;

            GL.GetInteger(GetPName.MaxVaryingFloats, out n);
            MaxVaryingFloats = n;
            GL.GetInteger(GetPName.MaxVaryingVectors, out n);
            MaxVaryingVectors = n;

            SeamlessCubemaps = IsExtensionAvailable("GL_ARB_seamless_cube_map");

            AnisotrophicFiltering = IsExtensionAvailable("GL_EXT_texture_filter_anisotropic");
            MaxAnisotrophyLevel = 0;
            if(AnisotrophicFiltering)
            {
                float maxAniso = 0;
                GL.GetFloat((GetPName)ExtTextureFilterAnisotropic.MaxTextureMaxAnisotropyExt, out maxAniso);
                MaxAnisotrophyLevel = maxAniso;
            }

        }
        
        public static bool IsExtensionAvailable(string extension)
        {
            Initialize();
            return extensions.Contains(extension);
        }

        public static bool TextureCompression { get; private set; }
        public static bool Instancing { get; private set; }
        public static int MaxVaryingFloats { get; private set; }
        public static int MaxVaryingVectors { get; private set; }
        public static bool SeamlessCubemaps { get; private set; }
        public static bool AnisotrophicFiltering { get; set; }
        public static float MaxAnisotrophyLevel { get; private set; }
    }
}
