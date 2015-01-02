using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hatzap.Assets;
using Hatzap.Utilities;
using OpenTK.Graphics.OpenGL;

namespace Hatzap.Shaders
{
    public static class ShaderManager
    {
        static Dictionary<string, ShaderProgram> programs = new Dictionary<string, ShaderProgram>();

        public static List<ShaderProgram> LoadCollection(string path)
        {
            using(var stream = PackageManager.GetStream(path))
            {
                return LoadCollection(XML.Read.FromStream<ShaderCollection>(stream));
            }
        }

        public static List<ShaderProgram> LoadCollection(ShaderCollection collection)
        {
            List<ShaderProgram> programs = new List<ShaderProgram>();

            foreach (var item in collection.ShaderPrograms)
            {
                programs.Add(Load(item));
            }

            return programs;
        }

        public static ShaderProgram Load(ShaderProgramInfo shaderProgram)
        {
            ShaderProgram program = new ShaderProgram(shaderProgram.Name);

            List<Shader> shaders = new List<Shader>();

            foreach (var shader in shaderProgram.Shaders)
            {
                Shader s = LoadShader(shader.Path, shader.Type);
                if(s != null)
                {
                    program.AttachShader(s);
                    shaders.Add(s);
                }
                else
                {
                    Debug.WriteLine("Unable to load {1} file {0}", shader.Path, shader.Type);
                }
            }
                        
            program.Link();

            foreach (var shader in shaders)
            {
                shader.Destroy();
            }

            programs.Add(program.Name, program);

            return program;
        }

        public static Shader LoadShader(string path, ShaderType type)
        {
            if (!File.Exists(path))
                return null;

            string source;

            using (StreamReader r = new StreamReader(path))
            {
                source = r.ReadToEnd();
            }

            Shader shader = new Shader(type);
            shader.ShaderSource(source);

            return shader;
        }


        public static ShaderProgram Get(string name)
        {
            ShaderProgram program = null;
            programs.TryGetValue(name, out program);
            return program;
        }
    }
}
