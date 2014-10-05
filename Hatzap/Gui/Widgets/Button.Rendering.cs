using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using Hatzap.Gui.Fonts;
using Hatzap.Shaders;
using Hatzap.Textures;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Hatzap.Gui.Widgets
{
    public partial class Button : Widget
    {
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

        public override GuiVertex[] GetVertices()
        {
            if (TextureRegion.Length < 9)
                return null;

            var vertices = new GuiVertex[3*2*9];

            int i = 0;

            Vector2 offset = Vector2.Zero;
            Vector2 quadSize = Vector2.Zero;

            // Top left
            quadSize = TextureRegion[0].Size;
            SetQuad(vertices, 0, ref i, ref offset, ref quadSize);

            // Top middle
            quadSize.X = Size.X;
            offset.X += TextureRegion[0].Size.X;
            SetQuad(vertices, 1, ref i, ref offset, ref quadSize);

            // Top right
            quadSize = TextureRegion[2].Size;
            offset.X += Size.X;
            SetQuad(vertices, 2, ref i, ref offset, ref quadSize);

            // Middle left
            quadSize.X = TextureRegion[3].Size.X;
            quadSize.Y = Size.Y;
            offset.X = 0;
            offset.Y = TextureRegion[0].Size.Y;
            SetQuad(vertices, 3, ref i, ref offset, ref quadSize);

            // Middle middle
            quadSize.X = Size.X;
            quadSize.Y = Size.Y;
            offset.X += TextureRegion[3].Size.X;
            offset.Y = TextureRegion[1].Size.Y;
            SetQuad(vertices, 4, ref i, ref offset, ref quadSize);

            // Middle right
            quadSize.X = TextureRegion[5].Size.X;
            quadSize.Y = Size.Y;
            offset.X += Size.X;
            offset.Y = TextureRegion[1].Size.Y;
            SetQuad(vertices, 5, ref i, ref offset, ref quadSize);

            // Bottom left
            quadSize.X = TextureRegion[6].Size.X;
            quadSize.Y = TextureRegion[6].Size.Y;
            offset.X = 0;
            offset.Y += Size.Y;
            SetQuad(vertices, 6, ref i, ref offset, ref quadSize);

            // Bottom middle
            quadSize.X = Size.X;
            quadSize.Y = TextureRegion[7].Size.Y;
            offset.X += TextureRegion[6].Size.X;
            SetQuad(vertices, 7, ref i, ref offset, ref quadSize);

            // Bottom right
            quadSize.X = TextureRegion[8].Size.X;
            quadSize.Y = TextureRegion[8].Size.Y;
            offset.X += Size.X;
            SetQuad(vertices, 8, ref i, ref offset, ref quadSize);

            return vertices;
        }

        void SetQuad(GuiVertex[] vertices, int tr, ref int i, ref Vector2 offset, ref Vector2 quadSize)
        {
            vertices[i].TexturePage = TextureRegion[tr].Page;
            vertices[i].TextureCoordinates = TextureRegion[tr].Offset;
            vertices[i++].Position = Position + offset;
            vertices[i].TexturePage = TextureRegion[tr].Page;
            vertices[i].TextureCoordinates = TextureRegion[tr].Offset + TextureRegion[tr].Size * new Vector2(0, 1);
            vertices[i++].Position = Position + quadSize * new Vector2(0, 1) + offset;
            vertices[i].TexturePage = TextureRegion[tr].Page;
            vertices[i].TextureCoordinates = TextureRegion[tr].Offset + TextureRegion[tr].Size;
            vertices[i++].Position = Position + quadSize + offset;

            vertices[i].TexturePage = TextureRegion[tr].Page;
            vertices[i].TextureCoordinates = TextureRegion[tr].Offset;
            vertices[i++].Position = Position + offset;
            vertices[i].TexturePage = TextureRegion[tr].Page;
            vertices[i].TextureCoordinates = TextureRegion[tr].Offset + TextureRegion[tr].Size;
            vertices[i++].Position = Position + quadSize + offset;
            vertices[i].TexturePage = TextureRegion[tr].Page;
            vertices[i].TextureCoordinates = TextureRegion[tr].Offset + TextureRegion[tr].Size * new Vector2(1, 0);
            vertices[i++].Position = Position + quadSize * new Vector2(1, 0) + offset;
        }

    }
}
