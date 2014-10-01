using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using OpenTK.Graphics.OpenGL;

namespace Hatzap.Textures
{
    public class TextureMeta
    {
        [XmlText]
        public string FileName { get; set; }
        [XmlAttribute]
        public TextureMinFilter TextureMinFilter { get; set; }
        [XmlAttribute]
        public TextureMagFilter TextureMagFilter { get; set; }
        [XmlAttribute]
        public float AnisotrophicFiltering { get; set; }
        [XmlAttribute]
        public bool Mipmaps { get; set; }
        [XmlAttribute]
        public PixelFormat PixelFormat { get; set; }
        [XmlAttribute]
        public PixelType PixelType { get; set; }
    }
}
