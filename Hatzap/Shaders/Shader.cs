using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.IO;
using System.Diagnostics;

namespace Hatzap.Shaders
{
    public class Shader
    {
        public int ID { get; protected set; }

        public ShaderType Type { get; protected set; }

        public Shader(ShaderType type)
        {
            Type = type;
            ID = GL.CreateShader(type);
        }

        public void ShaderSource(string source)
        {
            GL.ShaderSource(ID, source);
            Compile();
        }

        public void ShaderSource(Stream stream)
        {
            using(StreamReader reader = new StreamReader(stream))
            {
                GL.ShaderSource(ID, reader.ReadToEnd());
                Compile();
            }
        }

        void Compile()
        {
            GL.CompileShader(ID);

            int compileResult;
            GL.GetShader(ID, ShaderParameter.CompileStatus, out compileResult);
            if (compileResult != 1)
            {
                string info;
                GL.GetShaderInfoLog(ID, out info);

                throw new ShaderCompileErrorException(info);
            }
        }

        public void Destroy()
        {
            GL.DeleteShader(ID);
        }
    }
}
