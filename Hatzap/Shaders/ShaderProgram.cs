using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hatzap.Shaders
{
    public class ShaderProgram
    {
        public int ID { get; protected set; }

        public ShaderProgram()
        {
            ID = GL.CreateProgram();
        }

        public void AttachShader(Shader shader)
        {
            GL.AttachShader(ID, shader.ID);
        }

        public void AttachShaders(Shader[] shaders)
        {
            for (int i = 0; i < shaders.Length; i++)
            {
                GL.AttachShader(ID, shaders[i].ID);
            }
        }

        public void Enable()
        {
            GL.UseProgram(ID);
        }

        public void Link()
        {
            GL.LinkProgram(ID);

            string info;
            GL.GetProgramInfoLog(ID, out info);
            Debug.WriteLine(info);
        }
    }
}
