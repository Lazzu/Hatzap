using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Hatzap.Textures;
using OpenTK;

namespace Hatzap.Gui.Fonts
{
    public class Font : IXmlSerializable
    {
        public Texture Texture { get; set; }
        public string Face { get; set; }
        public Dictionary<char, Glyph> Glyphs { get; set; }

        public void LoadBMFont(string file)
        {
            string face;
            Glyphs = BMFont.GetGlyphsFromFile(file, out face);
            Face = face;

            
        }

        public GPUGlyph[] TextToVertices(string text, TextSettings settings)
        {
            if(Glyphs == null)
                return null;

            var vertices = new List<GPUGlyph>();

            Vector2 vOffset = Vector2.Zero;

            //vOffset.Y += settings.LineHeight / 2;

            int index = 0;
            List<int> linestarts = new List<int>();
            linestarts.Add(0);

            foreach(char c in text)
            {
                Glyph glyph;
                if(Glyphs.TryGetValue(c, out glyph))
                {
                    var gpuglyph = glyph.GetGPUGlyph(vOffset, settings.Size, settings.Weight, settings.Border, settings.Smooth, settings.Color, settings.BorderColor);

                    vertices.AddRange(gpuglyph);
                    vOffset.X += glyph.XAdvance * settings.GlyphSpacing;

                    index += gpuglyph.Length;

                    if(settings.MaxWidth > 0 && settings.MaxWidth <= vOffset.X)
                    {
                        vOffset.Y += settings.LineHeight;
                        vOffset.X = 0;

                        linestarts.Add(index);
                    }
                }
                else
                {
                    if(c == '\n')
                    {
                        vOffset.Y += settings.LineHeight;
                        vOffset.X = 0;

                        linestarts.Add(index);
                    }
                    else
                    {
                        vOffset.X += settings.Size * 0.5f;
                    }
                    
                }

                if (settings.MaxHeight > 0 && settings.MaxHeight <= vOffset.Y)
                    break;
            }

            if(settings.HorizontalAlignment != HorizontalAlignment.Left || settings.VerticalAlignment != VerticalAlignment.Top)
            {
                int lines = linestarts.Count;

                float voffset = 0;

                if (settings.VerticalAlignment == VerticalAlignment.Middle)
                {
                    var bottompos = vertices[vertices.Count - 1].Vertex.Y;
                    voffset = -bottompos / 2.0f;
                }
                else if (settings.VerticalAlignment == VerticalAlignment.Bottom)
                {
                    var bottompos = vertices[vertices.Count - 1].Vertex.Y;
                    voffset = -bottompos;
                }

                for(int i = 0; i < lines; i++)
                {
                    var linestart = linestarts[i];
                    int lineend = vertices.Count;

                    if(i+1 != lines)
                    {
                        lineend = linestarts[i + 1];
                    }

                    Vector2 offset = new Vector2(0, voffset);

                    if (settings.HorizontalAlignment == HorizontalAlignment.Centered)
                    {
                        var rightpos = vertices[lineend - 1].Vertex.X;
                        offset += new Vector2(-rightpos / 2, 0);
                    }
                    else if (settings.HorizontalAlignment == HorizontalAlignment.Right)
                    {
                        var rightpos = vertices[lineend - 1].Vertex.X;
                        offset += new Vector2(-rightpos, 0);
                    }

                    for (int vIndex = linestart; vIndex < lineend; vIndex++)
                    {
                        var tmp = vertices[vIndex];
                        tmp.Vertex += offset;
                        vertices[vIndex] = tmp;
                    }
                }
            }

            return vertices.ToArray();
        }

        public GPUGlyph[] TextToVertices(string text, float size, float weight, float border, float smooth, Vector4 color, Vector4 borderColor, float LineHeight, float glyphSpacing, float maxwidth, float maxheight)
        {
            return TextToVertices(text, new TextSettings()
            {
                Size = size,
                Weight = weight,
                Border = border,
                Smooth = smooth,
                Color = color,
                BorderColor = borderColor,
                LineHeight = LineHeight,
                GlyphSpacing = glyphSpacing,
                MaxWidth = maxwidth,
                MaxHeight = maxheight
            });
        }

        #region IXmlSerializable

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.Read();

            if(reader.Name != "Font")
                throw new Exception("Unable to deserialize font. Unknown format. Error: No font tag.");

            if(!reader.HasAttributes)
                throw new Exception("Unable to deserialize font. Unknown format. Error: Font tag has no attributes.");

            bool faceFound = false;

            while (reader.MoveToNextAttribute())
            {
                faceFound = reader.Name == "Face";

                if (faceFound)
                    break;    
            }

            if(!faceFound)
                throw new Exception("Unable to deserialize font. Unknown format. Error: Font tag has no Face attribute.");

            Face = reader.Value;

            reader.MoveToElement();

            while (reader.Read())
            {
                if (reader.IsStartElement())
                {
                    switch(reader.Name)
                    {
                        case "Glyph":
                            ReadGlyph(reader);
                            break;
                    }
                }
                else
                {
                    if(reader.Name == "Font")
                        break;
                }
            }
        }

        void ReadGlyph(XmlReader reader)
        {
            Glyph glyph = new Glyph();

            bool idfound = false;

            while (reader.MoveToNextAttribute())
            {
                idfound = reader.Name == "ID";

                if (idfound)
                    break;
            }

            if (!idfound)
                throw new Exception("Unable to deserialize font. Unknown format. Error: Glyph tag has no ID attribute.");

            glyph.Character = (char)Int32.Parse(reader.Value);

            reader.MoveToElement();

            CultureInfo culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            culture.NumberFormat.CurrencyDecimalSeparator = ".";

            while (reader.Read())
            {
                if (reader.IsStartElement())
                {
                    if(reader.Name == "Position")
                    {
                        var x = float.Parse(reader.GetAttribute("x"), NumberStyles.Any, culture);
                        var y = float.Parse(reader.GetAttribute("y"), NumberStyles.Any, culture);
                        glyph.Position = new Vector2(x, y);
                    }

                    if(reader.Name == "Size")
                    {
                        var x = float.Parse(reader.GetAttribute("x"), NumberStyles.Any, culture);
                        var y = float.Parse(reader.GetAttribute("y"), NumberStyles.Any, culture);
                        glyph.Size = new Vector2(x, y);
                    }

                    if(reader.Name == "Offset")
                    {
                        var x = float.Parse(reader.GetAttribute("x"), NumberStyles.Any, culture);
                        var y = float.Parse(reader.GetAttribute("y"), NumberStyles.Any, culture);
                        glyph.Offset = new Vector2(x, y);
                    }

                    if(reader.Name == "Scale")
                    {
                        var x = float.Parse(reader.GetAttribute("x"), NumberStyles.Any, culture);
                        var y = float.Parse(reader.GetAttribute("y"), NumberStyles.Any, culture);
                        glyph.Scale = new Vector2(x, y);
                    }

                    if(reader.Name == "XAdvance")
                    {
                        glyph.XAdvance = float.Parse(reader.GetAttribute("x"), NumberStyles.Any, culture);
                    }
                }
                else
                {
                    if (reader.Name == "Glyph")
                        break;
                }
            }

            Glyphs.Add(glyph.Character, glyph);
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
