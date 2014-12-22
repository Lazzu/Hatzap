using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Hatzap.Textures
{
    public class TextureArray : Texture
    {
        public TextureArray()
        {
            this.TextureTarget = TextureTarget.Texture2DArray;
        }

        /// <summary>
        /// Loads a TextureArray in to GPU memory
        /// </summary>
        /// <param name="meta"></param>
        public void Load(TextureMeta meta)
        {
            PixelInternalFormat = meta.PixelInternalFormat;
            Quality = meta.Quality;
            
            // Get last bound texture
            Texture last = null;
            Bound.TryGetValue(TextureTarget, out last);

            // Bind current texture
            Bind();

            Width = meta.Width;
            Height = meta.Height;

            string[] files = meta.FileName.Split(',');

            int filesCount = files.Length;

            GL.TexImage3D(TextureTarget, 0, PixelInternalFormat, Width, Height, filesCount, 0, meta.PixelFormat, meta.PixelType, IntPtr.Zero);

            for (int i = 0; i < filesCount; i++)
            {
                using(var bmp = new Bitmap(files[i]))
                {
                    BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                    GL.TexSubImage3D(TextureTarget, 0, 0, 0, i, Width, Height, 1, meta.PixelFormat, meta.PixelType, bitmapData.Scan0);

                    bmp.UnlockBits(bitmapData);
                }
            }

            // Restore previous state
            if (last != null) last.Bind();
            else UnBind();
        }
    }
}
