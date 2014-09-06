using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Hatzap.Shaders
{
    public class ShaderProgram
    {
        public int ID { get; protected set; }
        public string Name { get; protected set; }

        public ShaderProgram(string name)
        {
            Name = name;
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

        #region Uniforms

        Dictionary<string, int> uniforms = new Dictionary<string,int>();

        /// <summary>
        /// Tries to find the location of an uniform from this ShaderProgram object.
        /// </summary>
        /// <param name="name">Name of the uniform.</param>
        /// <returns>True if uniform was found. Otherwise false.</returns>
        public bool FindUniformLocation(string name)
        {
            int location = GetUniformLocation(name);
            
            // Check for valid response
            if (location == -1)
                return false;

            return true;
        }

        int GetUniformLocation(string name)
        {
            int location;
            if (!uniforms.TryGetValue(name, out location))
            {
                location = GL.GetUniformLocation(ID, name);

                uniforms.Add(name, location);
            }
            return location;
        }

        public void SendUniform(string name, float value)
        {
            GL.Uniform1(GetUniformLocation(name), value);
        }

        public void SendUniform(string name, double value)
        {
            GL.Uniform1(GetUniformLocation(name), value);
        }

        public void SendUniform(string name, int value)
        {
            GL.Uniform1(GetUniformLocation(name), value);
        }

        public void SendUniform(string name, ref Vector2 value)
        {
            GL.Uniform2(GetUniformLocation(name), ref value);
        }

        public void SendUniform(string name, ref Vector3 value)
        {
            GL.Uniform3(GetUniformLocation(name), ref value);
        }

        public void SendUniform(string name, ref Vector4 value)
        {
            GL.Uniform4(GetUniformLocation(name), ref value);
        }

        public void SendUniform(string name, ref Matrix2 value)
        {
            GL.UniformMatrix2(GetUniformLocation(name), false, ref value);
        }

        public void SendUniform(string name, ref Matrix3 value)
        {
            GL.UniformMatrix3(GetUniformLocation(name), false, ref value);
        }

        public void SendUniform(string name, ref Matrix4 value)
        {
            GL.UniformMatrix4(GetUniformLocation(name), false, ref value);
        }

        #endregion
    }
}
