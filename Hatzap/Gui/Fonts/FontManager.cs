using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hatzap.Textures;
using Hatzap.Utilities;
using OpenTK.Graphics.OpenGL;

namespace Hatzap.Gui.Fonts
{
    public static class FontManager
    {
        static Dictionary<string, Font> fonts = new Dictionary<string, Font>();

        public static void LoadCollection(string path)
        {
            LoadCollection(XML.Read.FromFile<FontCollection>(path));
        }

        public static void LoadCollection(FontCollection collection)
        {
            foreach (var item in collection.Fonts)
            {
                var font = new Font();
                font.LoadBMFont(item.FontDataFile);

                TextureMeta metadata = new TextureMeta()
                {
                    Precompressed = false,
                    FileName = item.FontTextureFile,
                    PixelInternalFormat = PixelInternalFormat.R8,
                    PixelFormat = PixelFormat.Bgra,
                    PixelType = PixelType.UnsignedByte,
                    Quality = new TextureQuality()
                    {
                        Anisotrophy = 1,
                        Filtering = TextureFiltering.Nearest,
                        TextureWrapMode_S = TextureWrapMode.Clamp,
                        TextureWrapMode_T = TextureWrapMode.Clamp,
                        Mipmaps = false
                    }
                };

                font.Texture = new Texture();

                throw new Exception("Fonts are broken until they adopt the new texture loading method.");

                /*font.Texture.Load(metadata);
                font.Texture.Bind();
                font.Texture.UpdateQuality();
                font.Texture.UnBind();*/
                
                fonts.Add(item.FontFamily, font);
            }
        }

        public static Font Get(string font)
        {
            Font fnt = null;
            fonts.TryGetValue(font, out fnt);
            return fnt;
        }
    }
}
