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
            Type = TextureType.Texture2D;
            PixelInternalFormat = PixelInternalFormat.Rgba;
            PixelFormat = PixelFormat.Bgra;
            PixelType = PixelType.UnsignedByte;
            Width = 0;
            Height = 0;
            Depth = 0;
            Precompressed = false;
            Quality = new TextureQuality();
        }

        /// <summary>
        /// The path to the file containing the texture data.
        /// </summary>
        [XmlIgnore]
        public string FileName { get; set; }

        [XmlAttribute]
        public TextureType Type { get; set; }

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
        public int Depth { get; set; }

        [XmlAttribute]
        public bool Precompressed { get; set; }

        [XmlAttribute]
        public bool PreMipmapped { get; set; }

        public TextureQuality Quality { get; set; }

        [XmlIgnore]
        protected List<string> filenames;

        [XmlIgnore]
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

        [XmlIgnore]
        public TextureMeta Copy
        {
            get
            {
                return new TextureMeta()
                {
                    FileName = FileName,
                    Type = Type,
                    PixelInternalFormat = PixelInternalFormat,
                    PixelFormat = PixelFormat,
                    PixelType = PixelType,
                    Width = Width,
                    Height = Height,
                    Depth = Depth,
                    Precompressed = Precompressed,
                    PreMipmapped = PreMipmapped,
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
