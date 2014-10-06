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

        public override void GetVertices(List<Vector2> vertices, List<Vector3> uv, List<Vector4> colors)
        {
            if (TextureRegion == null || TextureRegion.Length < 9)
                return;

            var v = new Vector2[3 * 2 * 9];
            var u = new Vector3[3 * 2 * 9];
            var c = new Vector4[3 * 2 * 9];

            int i = 0;

            Vector2 offset = Vector2.Zero;
            Vector2 quadSize = Vector2.Zero;

            float leftBorder = TextureRegion[0].Size.X;
            float rightBorder = TextureRegion[2].Size.X;
            float topBorder = TextureRegion[0].Size.Y;
            float bottomBorder = TextureRegion[8].Size.Y;

            // Top left
            quadSize = TextureRegion[0].Size;
            SetQuad(v, u, c, 0, ref i, ref offset, ref quadSize);

            // Top middle
            quadSize.X = Size.X - leftBorder - rightBorder;
            offset.X += TextureRegion[0].Size.X;
            SetQuad(v, u, c, 1, ref i, ref offset, ref quadSize);

            // Top right
            quadSize = TextureRegion[2].Size;
            offset.X += Size.X - leftBorder - rightBorder;
            SetQuad(v, u, c, 2, ref i, ref offset, ref quadSize);

            // Middle left
            quadSize.X = TextureRegion[3].Size.X;
            quadSize.Y = Size.Y - topBorder - bottomBorder;
            offset.X = 0;
            offset.Y = TextureRegion[0].Size.Y;
            SetQuad(v, u, c, 3, ref i, ref offset, ref quadSize);

            // Middle middle
            quadSize.X = Size.X - leftBorder - rightBorder;
            quadSize.Y = Size.Y - topBorder - bottomBorder;
            offset.X += TextureRegion[3].Size.X;
            offset.Y = TextureRegion[1].Size.Y;
            SetQuad(v, u, c, 4, ref i, ref offset, ref quadSize);

            // Middle right
            quadSize.X = TextureRegion[5].Size.X;
            quadSize.Y = Size.Y - topBorder - bottomBorder;
            offset.X += Size.X - leftBorder - rightBorder;
            offset.Y = TextureRegion[1].Size.Y;
            SetQuad(v, u, c, 5, ref i, ref offset, ref quadSize);

            // Bottom left
            quadSize.X = TextureRegion[6].Size.X;
            quadSize.Y = TextureRegion[6].Size.Y;
            offset.X = 0;
            offset.Y += Size.Y - topBorder - bottomBorder;
            SetQuad(v, u, c, 6, ref i, ref offset, ref quadSize);

            // Bottom middle
            quadSize.X = Size.X - leftBorder - rightBorder;
            quadSize.Y = TextureRegion[7].Size.Y;
            offset.X += TextureRegion[6].Size.X;
            SetQuad(v, u, c, 7, ref i, ref offset, ref quadSize);

            // Bottom right
            quadSize = TextureRegion[8].Size;
            offset.X += Size.X - leftBorder - rightBorder;
            SetQuad(v, u, c, 8, ref i, ref offset, ref quadSize);

            vertices.AddRange(v);
            uv.AddRange(u);
            colors.AddRange(c);
        }

        void SetQuad(Vector2[] vertices, Vector3[] uv, Vector4[] colors, int tr, ref int i, ref Vector2 offset, ref Vector2 quadSize)
        {
            var txr = TextureRegion[tr];

            var page = new Vector3(txr.Offset.X, txr.Offset.Y, txr.Page);
            var size = new Vector3(txr.Size.X, txr.Size.Y, 0);

            uv[i] = page;
            colors[i] = Color;
            vertices[i++]= Position + offset;
            uv[i] = page + size * new Vector3(0,1,0);
            colors[i] = Color;
            vertices[i++] = Position + quadSize * new Vector2(0, 1) + offset;
            uv[i] = page + size;
            colors[i] = Color;
            vertices[i++] = Position + quadSize + offset;

            uv[i] = page;
            colors[i] = Color;
            vertices[i++] = Position + offset;
            uv[i] = page + size;
            colors[i] = Color;
            vertices[i++] = Position + quadSize + offset;
            uv[i] = page + size * new Vector3(1,0,0);
            colors[i] = Color;
            vertices[i++] = Position + quadSize * new Vector2(1, 0) + offset;
        }

    }
}
