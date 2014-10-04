using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using OpenTK.Graphics.OpenGL;

namespace Hatzap.Shaders
{
    [Serializable]
    public class ShaderProgramInfo
    {
        public string Name { get; set; }

        [XmlArray("Shaders")]
        [XmlArrayItem("Shader")]
        public List<ShaderInfo> Shaders { get; set; }

        [XmlArray("Uniforms")]
        [XmlArrayItem("Uniform")]
        [XmlIgnore]
        public List<string> Uniforms { get; set; }

        public ShaderProgramInfo()
        {
            Shaders = new List<ShaderInfo>();
            Uniforms = new List<string>();
        }
    }

    [Serializable]
    public class ShaderInfo
    {
        [XmlAttribute]
        public ShaderType Type { get; set; }
        [XmlAttribute]
        public string Path { get; set; }
    }
}
