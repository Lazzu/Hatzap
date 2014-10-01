using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Hatzap.Gui.Widgets
{
    public class Image : Widget
    {
        public float BorderWidth { get; set; }

        public Image()
        {
            BorderWidth = 5;
        }

        public override GuiVertex[] GetVertices() 
        {
            if (TextureRegion == null || TextureRegion.Length < 1)
                return null;

            GuiVertex[] vertices = new GuiVertex[4];

            vertices[0].Position = Position + new Vector2(-BorderWidth, -BorderWidth);
            vertices[1].Position = Position + Size + new Vector2(BorderWidth, BorderWidth);
            vertices[2].Position = Position + new Vector2(0, Size.Y) + new Vector2(-BorderWidth, BorderWidth);
            vertices[3].Position = Position + Size + new Vector2(BorderWidth, BorderWidth);
            vertices[4].Position = Position + new Vector2(-BorderWidth, -BorderWidth);
            vertices[5].Position = Position + new Vector2(Size.X, 0) + new Vector2(BorderWidth, -BorderWidth);

            vertices[0].TextureCoordinates = TextureRegion[0].Offset;
            vertices[1].TextureCoordinates = TextureRegion[0].Offset + TextureRegion[0].Size;
            vertices[2].TextureCoordinates = TextureRegion[0].Offset + new Vector2(0, TextureRegion[0].Size.Y);
            vertices[3].TextureCoordinates = TextureRegion[0].Offset + TextureRegion[0].Size;
            vertices[4].TextureCoordinates = TextureRegion[0].Offset + new Vector2(0, 0);
            vertices[5].TextureCoordinates = TextureRegion[0].Offset + new Vector2(TextureRegion[0].Size.X, 0);

            vertices[0].TexturePage = TextureRegion[0].Page;
            vertices[1].TexturePage = TextureRegion[0].Page;
            vertices[2].TexturePage = TextureRegion[0].Page;
            vertices[3].TexturePage = TextureRegion[0].Page;
            vertices[4].TexturePage = TextureRegion[0].Page;
            vertices[5].TexturePage = TextureRegion[0].Page;

            return vertices;
        }
    }
}
