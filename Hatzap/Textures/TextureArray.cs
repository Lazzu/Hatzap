using System;
using System.Collections.Generic;
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

        public void Load(Bitmap[] bmps, SizedInternalFormat internalFormat, OpenTK.Graphics.OpenGL.PixelFormat format, PixelType type, TextureMinFilter minFilter, TextureMagFilter magFilter, float anisotrophy, bool mipmaps)
        {
            // Get last bound texture
            Texture last = null;
            Bound.TryGetValue(TextureTarget, out last);

            // Bind current texture
            Bind();

            GL.TexStorage3D(TextureTarget3d.Texture2DArray, 1, internalFormat, Width, Height, bmps.Length);

            for (int i = 0; i < bmps.Length; i++)
            {
                var transparent = IsTransparent(bmps[i]);

                BitmapData bitmapData = bmps[i].LockBits(new Rectangle(0, 0, bmps[i].Width, bmps[i].Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                // Reserve an empty image from GPU memory
                //GL.TexImage2D(TextureTarget, 0, internalFormat, Width, Height, 0, format, type, bitmapData.Scan0);


                GL.TexSubImage3D(TextureTarget, 0, 0, 0, i, Width, Height, 1, format, type, bitmapData.Scan0);

                bmps[i].UnlockBits(bitmapData);
            }

            if (mipmaps)
            {
                GenMipMaps();
            }

            TextureSettings(minFilter, magFilter, anisotrophy);

            // Restore previous state
            if (last != null) last.Bind();
            else UnBind();
        }

        public void Load(Bitmap[] bmps, SizedInternalFormat internalFormat, OpenTK.Graphics.OpenGL.PixelFormat format, PixelType type)
        {
            // Get last bound texture
            Texture last = null;
            Bound.TryGetValue(TextureTarget, out last);

            // Bind current texture
            Bind();

            GL.TexStorage3D(TextureTarget3d.Texture2DArray, 1, internalFormat, Width, Height, bmps.Length);

            for (int i = 0; i < bmps.Length; i++)
            {
                var transparent = IsTransparent(bmps[i]);

                BitmapData bitmapData = bmps[i].LockBits(new Rectangle(0, 0, bmps[i].Width, bmps[i].Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                // Reserve an empty image from GPU memory
                //GL.TexImage2D(TextureTarget, 0, internalFormat, Width, Height, 0, format, type, bitmapData.Scan0);


                GL.TexSubImage3D(TextureTarget, 0, 0, 0, i, Width, Height, 1, format, type, bitmapData.Scan0);

                bmps[i].UnlockBits(bitmapData);
            }

            // Restore previous state
            if (last != null) last.Bind();
            else UnBind();
        }
    }
}
