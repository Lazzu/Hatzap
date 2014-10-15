using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hatzap.Shaders;
using OpenTK;

namespace Hatzap.Rendering
{
    public interface IUniformData
    {
        void SendData(ShaderProgram program);
    }

    public abstract class UniformData<T> : IUniformData
    {
        public string Name;
        public T Data;
        public abstract void SendData(ShaderProgram program);
    }

    public class UniformDataFloat : UniformData<float>
    {   
        public override void SendData(ShaderProgram program)
        {
            program.SendUniform(Name, Data);
        }
    }

    public class UniformDataDouble : UniformData<double>
    {
        public override void SendData(ShaderProgram program)
        {
            program.SendUniform(Name, Data);
        }
    }

    public class UniformDataInt : UniformData<int>
    {
        public override void SendData(ShaderProgram program)
        {
            program.SendUniform(Name, Data);
        }
    }

    public class UniformDataUInt : UniformData<uint>
    {
        public override void SendData(ShaderProgram program)
        {
            program.SendUniform(Name, Data);
        }
    }

    public class UniformDataVector2 : UniformData<Vector2>
    {
        public override void SendData(ShaderProgram program)
        {
            program.SendUniform(Name, ref Data);
        }
    }

    public class UniformDataVector3 : UniformData<Vector3>
    {
        public override void SendData(ShaderProgram program)
        {
            program.SendUniform(Name, ref Data);
        }
    }

    public class UniformDataVector4 : UniformData<Vector4>
    {
        public override void SendData(ShaderProgram program)
        {
            program.SendUniform(Name, ref Data);
        }
    }

    public class UniformDataMatrix2 : UniformData<Matrix2>
    {
        public override void SendData(ShaderProgram program)
        {
            program.SendUniform(Name, ref Data);
        }
    }

    public class UniformDataMatrix3 : UniformData<Matrix3>
    {
        public override void SendData(ShaderProgram program)
        {
            program.SendUniform(Name, ref Data);
        }
    }

    public class UniformDataMatrix4 : UniformData<Matrix4>
    {
        public override void SendData(ShaderProgram program)
        {
            program.SendUniform(Name, ref Data);
        }
    }
}
