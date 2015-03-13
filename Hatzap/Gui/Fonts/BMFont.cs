using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hatzap.Assets;
using OpenTK;

namespace Hatzap.Gui.Fonts
{
    public static class BMFont
    {
        static Dictionary<char, Glyph> GetGlyphs(StreamReader stream, out string FontFace)
        {
            var glyphs = new Dictionary<char, Glyph>();

            string line;
            FontFace = "";
                        
            // Import glyphs
            while((line = stream.ReadLine()) != null)
            {
                var parts = line.Split(' ');

                parts[0] = parts[0].ToLower();

                if(parts[0] == "info")
                {
                    ParseInfo(parts, out FontFace);
                }
                else if(parts[0] == "char")
                {
                    Glyph glyph;
                    ParseGlyph(parts, out glyph);
                    glyphs.Add(glyph.Character, glyph);
                }
            }

            Glyph em = glyphs['M'];

            float width = 1.0f / em.Size.X;
            float height = 1.0f / em.Size.Y;
            
            // Normalize glyph data

            foreach (var kvp in glyphs)
            {
                Glyph glyph = kvp.Value;
                glyph.Scale = new Vector2(width, height);
            }

            return glyphs;
        }

        static void ParseInfo(string[] parts, out string FontFace)
        {
            FontFace = "";

            for(int i = 1; i < parts.Length; i++)
            {
                var kvp = parts[i].Split('=');

                if(kvp[0].ToLower() == "face")
                {
                    FontFace = kvp[1].Trim('"');
                }
            }

        }

        static void ParseGlyph(string[] parts, out Glyph glyph)
        {
            glyph = new Glyph();

            CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.CurrencyDecimalSeparator = ".";

            Vector2 pos = Vector2.Zero, size = Vector2.Zero, offset = Vector2.Zero;

            for (int i = 1; i < parts.Length; i++)
            {
                if (parts[i] == string.Empty)
                    continue;

                var kvp = parts[i].Split('=');

                kvp[0] = kvp[0].ToLower();

                if (kvp[0] == "id")
                {
                    glyph.Character = (char)Int32.Parse(kvp[1]);
                }
                else if (kvp[0] == "x")
                {
                    pos.X = Int32.Parse(kvp[1]);
                }
                else if (kvp[0] == "y")
                {
                    pos.Y = Int32.Parse(kvp[1]);
                }
                else if (kvp[0] == "width")
                {
                    size.X = Int32.Parse(kvp[1]);
                }
                else if (kvp[0] == "height")
                {
                    size.Y = Int32.Parse(kvp[1]);
                }
                else if (kvp[0] == "xoffset")
                {
                    offset.X = float.Parse(kvp[1], NumberStyles.Any, ci);
                }
                else if (kvp[0] == "yoffset")
                {
                    offset.Y = -float.Parse(kvp[1], NumberStyles.Any, ci);
                }
                else if (kvp[0] == "xadvance")
                {
                    glyph.XAdvance = float.Parse(kvp[1], NumberStyles.Any, ci);
                }
            }

            glyph.Position = pos;
            glyph.Size = size;
            glyph.Offset = offset;
        }

        static public Dictionary<char, Glyph> GetGlyphs(Stream stream, out string FontFace)
        {
            using (StreamReader r = new StreamReader(stream))
            {
                return GetGlyphs(r, out FontFace);
            }
        }

        static public Dictionary<char, Glyph> GetGlyphs(string str, out string FontFace)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            using(MemoryStream ms = new MemoryStream(bytes))
            {
                return GetGlyphs(ms, out FontFace);
            }
        }

        static public Dictionary<char, Glyph> GetGlyphsFromFile(string file, out string FontFace)
        {
            return GetGlyphs(PackageManager.GetStream(file), out FontFace);
        }


    }
}
