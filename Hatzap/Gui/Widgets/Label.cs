using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hatzap.Gui.Fonts;
using Hatzap.Shaders;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Hatzap.Gui.Widgets
{
    public class Label : Widget
    {
        GuiText text = new GuiText();

        public Font Font { get { return text.Font; } set { text.Font = value; } }
        public string Text { get { return text.Text; } set { text.Text = value; } }

        public int TextBaseline { get; set; }

        public GuiText GuiText { get { return text; } }

        public Label()
        {
            Font = FontManager.Get("OpenSans-Regular");
            TextBaseline = (int)text.FontSize;
            GuiText.Smooth = 0.6f;
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

        public override string CustomRenderLayer { get { return "GuiText"; } }

        public override void CustomRender()
        {
            var shader = ShaderManager.Get("Text");
            shader.Enable();

            text.HorizontalAlignment = HorizontalAlignment.Centered;

            var textureSize = new Vector2(text.Font.Texture.Width, text.Font.Texture.Height);

            Matrix4 mvp = Matrix4.CreateTranslation(Position.X + Size.X / 2, Position.Y + TextBaseline, 0) * GuiRoot.Root.Projection;

            shader.SendUniform("MVP", ref mvp);
            shader.SendUniform("textureSize", ref textureSize);
            GL.ActiveTexture(TextureUnit.Texture0);

            text.Font.Texture.Bind();

            text.Draw(shader);

            shader.Disable();
        }
    }
}
