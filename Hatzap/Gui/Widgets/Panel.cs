using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Hatzap.Gui.Widgets
{
    public class Panel : WidgetGroup
    {
        public override int Capacity
        {
            get
            {
                return 1;
            }
        }



        public Panel()
        {

        }

        public override void GetVertices(List<Vector2> vertices, List<Vector3> uv, List<Vector4> colors)
        {
            if (TextureRegion == null || TextureRegion.Length < 9)
                return;

            int cells = 1;

            bool hasLeft = false;
            bool hasTop = false;
            bool hasBottom = false;
            bool hasRight = false;
            
            float leftBorder = TextureRegion[0].Size.X;
            float rightBorder = TextureRegion[2].Size.X;
            float topBorder = TextureRegion[0].Size.Y;
            float bottomBorder = TextureRegion[8].Size.Y;

            if (Anchor.Directions[Anchors.AnchorDirection.Left] != Anchors.AnchorType.Snap)
            {
                hasLeft = true;
                cells++;
            }
            else
            {
                leftBorder = 0;
            }

            if (Anchor.Directions[Anchors.AnchorDirection.Right] != Anchors.AnchorType.Snap)
            {
                hasRight = true;
                cells++;
            }
            else
            {
                rightBorder = 0;
            }

            if (Anchor.Directions[Anchors.AnchorDirection.Top] != Anchors.AnchorType.Snap)
            {
                hasTop = true;
                cells++;
            }
            else
            {
                topBorder = 0;
            }

            if (Anchor.Directions[Anchors.AnchorDirection.Bottom] != Anchors.AnchorType.Snap)
            {
                hasBottom = true;
                cells++;
            }
            else
            {
                bottomBorder = 0;
            }

            if (hasTop && hasLeft)
                cells++;

            if (hasTop && hasRight)
                cells++;

            if (hasBottom && hasLeft)
                cells++;

            if (hasBottom && hasRight)
                cells++;

            var v = new Vector2[3 * 2 * cells];
            var u = new Vector3[3 * 2 * cells];
            var c = new Vector4[3 * 2 * cells];

            int i = 0;

            Vector2 offset = Vector2.Zero;
            Vector2 quadSize = Vector2.Zero;

            // Top left
            quadSize.X = leftBorder;
            quadSize.Y = topBorder;
            if (hasLeft && hasTop)
            {
                SetQuad(v, u, c, 0, ref i, ref offset, ref quadSize);
            }

            // Top middle
            quadSize.X = Size.X - leftBorder - rightBorder;
            offset.X += leftBorder;
            if(hasTop)
            {
                SetQuad(v, u, c, 1, ref i, ref offset, ref quadSize);
            }

            // Top right
            quadSize.X = rightBorder;
            offset.X += Size.X - leftBorder - rightBorder;
            if (hasTop && hasRight)
            {
                SetQuad(v, u, c, 2, ref i, ref offset, ref quadSize);
            }

            // Middle left
            quadSize.X = leftBorder;
            quadSize.Y = Size.Y - topBorder - bottomBorder;
            offset.X = 0;
            offset.Y = topBorder;
            if(hasLeft)
            {
                SetQuad(v, u, c, 3, ref i, ref offset, ref quadSize);
            }
            
            // Middle middle
            offset.X += leftBorder;
            quadSize.X = Size.X - leftBorder - rightBorder;
            SetQuad(v, u, c, 4, ref i, ref offset, ref quadSize);

            offset.X += Size.X - leftBorder - rightBorder;
            quadSize.X = rightBorder;
            if(hasRight)
            {
                // Middle right
                SetQuad(v, u, c, 5, ref i, ref offset, ref quadSize);
            }
            

            // Bottom left
            quadSize.X = leftBorder;
            quadSize.Y = bottomBorder;
            offset.X = 0;
            offset.Y = Size.Y - bottomBorder;
            if (hasBottom && hasLeft)
            {
                SetQuad(v, u, c, 6, ref i, ref offset, ref quadSize);
            }
            

            // Bottom middle
            quadSize.X = Size.X - leftBorder - rightBorder;
            offset.X += leftBorder;
            if(hasBottom)
            {
                SetQuad(v, u, c, 7, ref i, ref offset, ref quadSize);
            }
            

            // Bottom right
            quadSize.X = rightBorder;
            offset.X += Size.X - leftBorder - rightBorder;
            if (hasBottom && hasRight)
            {
                SetQuad(v, u, c, 8, ref i, ref offset, ref quadSize);
            }

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
            vertices[i++] = Position + offset;
            uv[i] = page + size * new Vector3(0, 1, 0);
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
            uv[i] = page + size * new Vector3(1, 0, 0);
            colors[i] = Color;
            vertices[i++] = Position + quadSize * new Vector2(1, 0) + offset;
        }
    }
}
