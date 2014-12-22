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
        public TextureMeta()
        {
            FileName = "Unnamed";
            Name = "Unnamed";
            PixelInternalFormat = PixelInternalFormat.Rgba;
            PixelFormat = PixelFormat.Bgra;
            PixelType = PixelType.UnsignedByte;
            Width = 0;
            Height = 0;
            Precompressed = false;
            Quality = new TextureQuality();
        }

        [XmlText]
        public string FileName { get; set; }

        [XmlText]
        public string Name { get; set; }
        
        [XmlAttribute]
        public PixelInternalFormat PixelInternalFormat { get; set; }

        [XmlAttribute]
        public PixelFormat PixelFormat { get; set; }

        [XmlAttribute]
        public PixelType PixelType { get; set; }

        [XmlAttribute]
        public int Width { get; set; }

        [XmlAttribute]
        public int Height { get; set; }

        [XmlAttribute]
        public bool Precompressed { get; set; }

        [XmlAttribute]
        public TextureQuality Quality { get; set; }


        protected List<string> filenames;
        internal List<string> FileNames
        {
            get
            {
                if (filenames == null)
                {
                    filenames = new List<string>();
                    filenames.AddRange(FileName.Split(','));
                }
                return filenames;
            }
        }

        public TextureMeta Copy
        {
            get
            {
                return new TextureMeta()
                {
                    FileName = FileName,
                    Name = Name,
                    PixelInternalFormat = PixelInternalFormat,
                    PixelFormat = PixelFormat,
                    PixelType = PixelType,
                    Width = Width,
                    Height = Height,
                    Precompressed = Precompressed,
                    Quality = Quality,
                    filenames = filenames,
                };
            }
        }

        public static TextureMeta Generate()
        {
            return new TextureMeta();
        }


    }
}
