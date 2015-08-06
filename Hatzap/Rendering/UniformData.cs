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
        string Name { get; }

        void SendData(ShaderProgram program);

        IUniformData Copy();
    }

    public abstract class UniformData<T> : IUniformData
    {
        public string Name { get; set; }
        public T Data;
        public abstract void SendData(ShaderProgram program);

        public abstract IUniformData Copy();
        public override bool Equals(object obj)
        {
            var d = obj as UniformData<T>;
            if (d == null)
                return false;
            return Name == d.Name && Data.Equals(d.Data);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                return Data.GetHashCode() + Name.GetHashCode();
            }
        }
    }

    public class UniformDataFloat : UniformData<float>
    {
        public override void SendData(ShaderProgram program)
        {
            program.SendUniform(Name, Data);
        }

        public override IUniformData Copy()
        {
            return new UniformDataFloat()
            {
                Name = Name,
                Data = Data
            } as IUniformData;
        }
    }

    public class UniformDataDouble : UniformData<double>
    {
        public override void SendData(ShaderProgram program)
        {
            program.SendUniform(Name, Data);
        }

        public override IUniformData Copy()
        {
            return new UniformDataDouble()
            {
                Name = Name,
                Data = Data
            } as IUniformData;
        }
    }

    public class UniformDataInt : UniformData<int>
    {
        public override void SendData(ShaderProgram program)
        {
            program.SendUniform(Name, Data);
        }

        public override IUniformData Copy()
        {
            return new UniformDataDouble()
            {
                Name = Name,
                Data = Data
            } as IUniformData;
        }
    }

    public class UniformDataUInt : UniformData<uint>
    {
        public override void SendData(ShaderProgram program)
        {
            program.SendUniform(Name, Data);
        }

        public override IUniformData Copy()
        {
            return new UniformDataUInt()
            {
                Name = Name,
                Data = Data
            } as IUniformData;
        }
    }

    public class UniformDataVector2 : UniformData<Vector2>
    {
        public override void SendData(ShaderProgram program)
        {
            program.SendUniform(Name, ref Data);
        }

        public override IUniformData Copy()
        {
            return new UniformDataVector2()
            {
                Name = Name,
                Data = Data
            } as IUniformData;
        }
    }

    public class UniformDataVector3 : UniformData<Vector3>
    {
        public override void SendData(ShaderProgram program)
        {
            program.SendUniform(Name, ref Data);
        }

        public override IUniformData Copy()
        {
            return new UniformDataVector3()
            {
                Name = Name,
                Data = Data
            } as IUniformData;
        }
    }

    public class UniformDataVector4 : UniformData<Vector4>
    {
        public override void SendData(ShaderProgram program)
        {
            program.SendUniform(Name, ref Data);
        }

        public override IUniformData Copy()
        {
            return new UniformDataVector4()
            {
                Name = Name,
                Data = Data
            } as IUniformData;
        }
    }

    public class UniformDataMatrix2 : UniformData<Matrix2>
    {
        public override void SendData(ShaderProgram program)
        {
            program.SendUniform(Name, ref Data);
        }

        public override IUniformData Copy()
        {
            return new UniformDataMatrix2()
            {
                Name = Name,
                Data = Data
            } as IUniformData;
        }
    }

    public class UniformDataMatrix3 : UniformData<Matrix3>
    {
        public override void SendData(ShaderProgram program)
        {
            program.SendUniform(Name, ref Data);
        }

        public override IUniformData Copy()
        {
            return new UniformDataMatrix3()
            {
                Name = Name,
                Data = Data
            } as IUniformData;
        }
    }

    public class UniformDataMatrix4 : UniformData<Matrix4>
    {
        public override void SendData(ShaderProgram program)
        {
            program.SendUniform(Name, ref Data);
        }

        public override IUniformData Copy()
        {
            return new UniformDataMatrix4()
            {
                Name = Name,
                Data = Data
            } as IUniformData;
        }
    }
}
