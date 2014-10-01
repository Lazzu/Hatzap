using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hatzap.Gui.Fonts;

namespace Hatzap.Gui.Widgets
{
    public class Label : Widget
    {
        GuiText text = new GuiText();

        public Font Font { get { return text.Font; } set { text.Font = value; } }
        public string Text { get { return text.Text; } set { text.Text = value; } }

        public Label()
        {
            Font = FontManager.Get(GuiSettings.Current.DefaultFont);
        }

        public Label(Font font)
        {
            Font = font;
        }

        public Label(string text) : this()
        {
            this.text.Text = text;
        }

        public Label(string text, Font font)
        {
            Font = font;
            this.text.Text = text;
        }

        public override GuiVertex[] GetVertices()
        {
            return base.GetVertices();
        }
    }
}
