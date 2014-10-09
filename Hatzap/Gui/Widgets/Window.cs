using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Hatzap.Gui.Widgets
{
    public class Window : WidgetGroup
    {

        public float TitleHeight { get; set; }

        public Vector4 TitleColor { get; set; }

        public Window()
        {
            TitleColor = new Vector4(1, 1, 1, 1);
        }

        public override void GetVertices(List<Vector2> vertices, List<Vector3> uv, List<Vector4> colors)
        {
            if (TextureRegion == null || TextureRegion.Length < 12)
                return;

            var v = new Vector2[3 * 2 * 12];
            var u = new Vector3[3 * 2 * 12];
            var c = new Vector4[3 * 2 * 12];

            int i = 0;

            Vector2 offset = Vector2.Zero;
            Vector2 quadSize = Vector2.Zero;

            float leftBorder = TextureRegion[0].Size.X;
            float rightBorder = TextureRegion[2].Size.X;
            float topBorder = TextureRegion[0].Size.Y;
            float bottomBorder = TextureRegion[8].Size.Y;

            Vector4 color = TitleColor;

            // Top left
            quadSize = TextureRegion[0].Size;
            SetQuad(v, u, c, 0, ref i, ref offset, ref quadSize, ref color);

            // Top middle
            quadSize.X = Size.X;
            offset.X += TextureRegion[0].Size.X;
            SetQuad(v, u, c, 1, ref i, ref offset, ref quadSize, ref color);

            // Top right
            quadSize = TextureRegion[2].Size;
            offset.X += Size.X;
            SetQuad(v, u, c, 2, ref i, ref offset, ref quadSize, ref color);

            // Middle left
            quadSize.X = TextureRegion[3].Size.X;
            quadSize.Y = TitleHeight;
            offset.X = 0;
            offset.Y = TextureRegion[0].Size.Y;
            SetQuad(v, u, c, 3, ref i, ref offset, ref quadSize, ref color);

            // Middle middle
            quadSize.X = Size.X;
            quadSize.Y = TitleHeight;
            offset.X += TextureRegion[3].Size.X;
            SetQuad(v, u, c, 4, ref i, ref offset, ref quadSize, ref color);

            // Middle right
            quadSize.X = TextureRegion[5].Size.X;
            quadSize.Y = TitleHeight;
            offset.X += Size.X;
            SetQuad(v, u, c, 5, ref i, ref offset, ref quadSize, ref color);

            // Bottom left
            quadSize.X = TextureRegion[6].Size.X;
            quadSize.Y = Size.Y;
            offset.X = 0;
            offset.Y += TitleHeight;
            SetQuad(v, u, c, 6, ref i, ref offset, ref quadSize);

            // Bottom middle
            quadSize = Size;
            offset.X += TextureRegion[6].Size.X;
            SetQuad(v, u, c, 7, ref i, ref offset, ref quadSize);

            // Bottom right
            quadSize.X = TextureRegion[8].Size.X;
            quadSize.Y = Size.Y;
            offset.X += Size.X;
            SetQuad(v, u, c, 8, ref i, ref offset, ref quadSize);

            // Bottom left
            quadSize = TextureRegion[9].Size;
            offset.X = 0;
            offset.Y += Size.Y;
            SetQuad(v, u, c, 9, ref i, ref offset, ref quadSize);

            // Bottom middle
            quadSize.X = Size.X;
            //quadSize.Y = TextureRegion[10].Size.Y;
            offset.X += TextureRegion[0].Size.X;
            SetQuad(v, u, c, 10, ref i, ref offset, ref quadSize);

            // Bottom right
            quadSize = TextureRegion[11].Size;
            offset.X += Size.X;
            SetQuad(v, u, c, 11, ref i, ref offset, ref quadSize);

            vertices.AddRange(v);
            uv.AddRange(u);
            colors.AddRange(c);
        }

        void SetQuad(Vector2[] vertices, Vector3[] uv, Vector4[] colors, int tr, ref int i, ref Vector2 offset, ref Vector2 quadSize)
        {
            var color = Color;
            SetQuad(vertices, uv, colors, tr, ref i, ref offset, ref quadSize, ref color);
        }

        void SetQuad(Vector2[] vertices, Vector3[] uv, Vector4[] colors, int tr, ref int i, ref Vector2 offset, ref Vector2 quadSize, ref Vector4 color)
        {
            var txr = TextureRegion[tr];

            var page = new Vector3(txr.Offset.X, txr.Offset.Y, txr.Page);
            var size = new Vector3(txr.Size.X, txr.Size.Y, 0);

            uv[i] = page;
            colors[i] = color;
            vertices[i++] = Position + offset;
            uv[i] = page + size * new Vector3(0, 1, 0);
            colors[i] = color;
            vertices[i++] = Position + quadSize * new Vector2(0, 1) + offset;
            uv[i] = page + size;
            colors[i] = color;
            vertices[i++] = Position + quadSize + offset;

            uv[i] = page;
            colors[i] = color;
            vertices[i++] = Position + offset;
            uv[i] = page + size;
            colors[i] = color;
            vertices[i++] = Position + quadSize + offset;
            uv[i] = page + size * new Vector3(1, 0, 0);
            colors[i] = color;
            vertices[i++] = Position + quadSize * new Vector2(1, 0) + offset;
        }

    }
}
