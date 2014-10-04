using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hatzap.Gui.Fonts;
using Hatzap.Textures;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Hatzap.Gui.Widgets
{
    public partial class Button
    {
        public string Text { get { return text.Text; } set { text.Text = value; } }
        public Vector4 Color { get { return text.Color; } set { text.Color = value; } }

        public int TextBaseline { get; set; }

        readonly GuiText text;

        public GuiText GuiText { get { return text; } }

        public Button()
        {
            text = new GuiText();
            text.Font = FontManager.Get("OpenSans-Regular");
            text.FontSize = 20;
            text.Smooth = 0.5f;
            text.Text = "Button";

            TextBaseline = 35;
        }
    }
}
