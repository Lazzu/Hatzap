using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Hatzap.Gui.Fonts
{
    public class TextSettings
    {
        public float Size { get; set; }
        public float Weight { get; set; }
        public float Border { get; set; }
        public float Smooth { get; set; }
        public float LineHeight { get; set; }
        public float GlyphSpacing { get; set; }
        public float MaxWidth { get; set; }
        public float MaxHeight { get; set; }
        public Vector4 Color { get; set; }
        public Vector4 BorderColor { get; set; }
        public HorizontalAlignment HorizontalAlignment { get; set; }
        public VerticalAlignment VerticalAlignment { get; set; }

        public TextSettings()
        {
            Size = 12.0f;
            Weight = 1.0f;
            Border = 0.0f;
            Smooth = 0.2f;
            LineHeight = 1.0f;
            GlyphSpacing = 1.0f;
            MaxWidth = 0;
            MaxHeight = 0;
            Color = Vector4.One;
            BorderColor = Vector4.UnitW;
            VerticalAlignment = Fonts.VerticalAlignment.Top;
            HorizontalAlignment = Fonts.HorizontalAlignment.Left;
        }

        public static TextSettings Default
        {
            get
            {
                return new TextSettings();
            }
        }
    }
}
