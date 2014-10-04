using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Hatzap.Shaders
{
    [Serializable]
    public class ShaderCollection
    {
        [XmlArray("ShaderPrograms")]
        [XmlArrayItem("ShaderProgram")]
        public List<ShaderProgramInfo> ShaderPrograms { get; set; }

        public ShaderCollection()
        {
            ShaderPrograms = new List<ShaderProgramInfo>();
        }
    }
}
