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
                font.Texture = new Texture();
                font.Texture.PixelInternalFormat = PixelInternalFormat.R8;
                font.Texture.Load(new Bitmap(item.FontTextureFile), PixelFormat.Bgra, PixelType.UnsignedByte);
                font.Texture.Bind();
                font.Texture.TextureSettings(TextureMinFilter.Linear, TextureMagFilter.Linear, 0);

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
