using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Hatzap.Gui.Fonts
{
    public class Glyph
    {
        public char Character { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
        public Vector2 Offset { get; set; }
        public float XAdvance { get; set; }

        public Vector2 Scale { get; set; }

        internal Glyph()
        {

        }

        public Glyph(char character, Vector2 pos, Vector2 size, Vector2 offset, float xadv)
        {
            this.Character = character;
            this.Position = pos;
            this.Size = size;
            this.Offset = offset;
            this.XAdvance = xadv;
        }

        /// <summary>
        /// Gets GPU compatible array of vertices for this glyph.
        /// </summary>
        /// <param name="vOffset">Vertex offset</param>
        /// <param name="scale">Vertex scale</param>
        /// <param name="weight">Font weight</param>
        /// <param name="border">Font border size</param>
        /// <param name="color">Font color</param>
        /// <returns>Array of vertices</returns>
        public GPUGlyph[] GetGPUGlyph(Vector2 vOffset, float scale, float weight, float border, float smooth, ref Vector4 color, ref Vector4 borderColor)
        {
            return GetGPUGlyph(ref vOffset, scale, weight, border, smooth, ref color, ref borderColor);
        }

        /// <summary>
        /// Gets GPU compatible array of vertices for this glyph.
        /// </summary>
        /// <param name="vOffset">Vertex offset</param>
        /// <param name="scale">Vertex scale</param>
        /// <param name="weight">Font weight</param>
        /// <param name="border">Font border size</param>
        /// <param name="color">Font color</param>
        /// <returns>Array of vertices</returns>
        public GPUGlyph[] GetGPUGlyph(Vector2 vOffset, float scale, float weight, float border, float smooth, Vector4 color, Vector4 borderColor)
        {
            return GetGPUGlyph(ref vOffset, scale, weight, border, smooth, ref color, ref borderColor);
        }

        /// <summary>
        /// Gets GPU compatible array of vertices for this glyph. Automatically advances vOffset.X by amount of XAdvance.
        /// </summary>
        /// <param name="vOffset">Vertex offset. Gets automatically advanced by amount of XAdvance</param>
        /// <param name="scale">Vertex scale</param>
        /// <param name="weight">Font weight</param>
        /// <param name="border">Font border size</param>
        /// <param name="color">Font color</param>
        /// <returns>Array of vertices</returns>
        public GPUGlyph[] GetGPUGlyph(ref Vector2 vOffset, float scale, float weight, float border, float smooth, Vector4 color, Vector4 borderColor)
        {
            return GetGPUGlyph(ref vOffset, scale, weight, border, smooth, ref color, ref borderColor);
        }

        /// <summary>
        /// Gets GPU compatible array of vertices for this glyph. Automatically advances vOffset.X by amount of XAdvance.
        /// </summary>
        /// <param name="vOffset">Vertex offset. Gets automatically advanced by amount of XAdvance</param>
        /// <param name="size">Vertex scale</param>
        /// <param name="weight">Font weight</param>
        /// <param name="border">Font border size</param>
        /// <param name="color">Font color</param>
        /// <returns>Array of vertices</returns>
        public GPUGlyph[] GetGPUGlyph(ref Vector2 vOffset, float size, float weight, float border, float smooth, ref Vector4 color, ref Vector4 borderColor)
        {
            var gpuGlyph = new GPUGlyph[]
            {
                new GPUGlyph() {
                    Vertex = (new Vector2(0,0) * Size + Offset + vOffset) * size * Scale,
                    TCoord = Position,
                    Color = color,
                    BorderColor = borderColor
                },
                new GPUGlyph() {
                    Vertex = (new Vector2(0,1) * Size + Offset + vOffset) * size * Scale,
                    TCoord = Position + new Vector2(0, Size.Y),
                    Color = color,
                    BorderColor = borderColor
                },
                new GPUGlyph() {
                    Vertex = (new Vector2(1,0) * Size + Offset + vOffset) * size * Scale,
                    TCoord = Position + new Vector2(Size.X, 0),
                    Color = color,
                    BorderColor = borderColor
                },
                new GPUGlyph() {
                    Vertex = (new Vector2(1,1) * Size + Offset + vOffset) * size * Scale,
                    TCoord = Position + Size,
                    Color = color,
                    BorderColor = borderColor
                }
            };

            vOffset.X += XAdvance;

            return gpuGlyph;
        }
    }
}
