using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace Hatzap.Textures
{
    public class CubeTexture : Texture
    {
        public CubeTexture(TextureMeta meta) : base(meta)
        {
            
        }

        public override void Load(byte[] rawBytes, TextureMeta meta)
        {
            if (meta.Type != TextureType.TextureCubemap)
            {
                throw new ArgumentException("Texture type must be TextureCubemap. CubeTexture class does not support loading other types.");
            }

            Width = meta.Width;
            Height = meta.Height;
            PixelInternalFormat = meta.PixelInternalFormat;

            int levels = 1;

            if (meta.PreMipmapped)
            {
                int size = Math.Max(Width, Height);
                levels += (int)Math.Floor(Math.Log(size, 2));
            }

            Bind();

            int bpp = 0;
            int w = Width;
            int h = Height;

            if (!meta.Precompressed)
            {
                bpp = GetBpp(meta.PixelFormat);

                if (bpp == 0) throw new NotImplementedException("The selected PixelFormat used for loading texture to file not yet supported.");
            }

            int srcOffset = 0;

            GL.TexParameter(TextureTarget, TextureParameterName.TextureMaxLevel, levels - 1);

            for (int i = 0; i < levels; i++)
            {
                // How many bytes on this level?
                int amount = w * h * bpp;

                // Copy bytes to separate buffer
                var levelBytes = new byte[amount];

                for (var direction = 0; direction < 6; direction++ )
                {
                    Array.Copy(rawBytes, srcOffset, levelBytes, 0, amount);
                    //Buffer.BlockCopy(rawBytes, srcOffset, levelBytes, 0, amount);

                    // Upload bytes to the GPU

                    TextureTarget directionTarget;

                    switch (direction)
                    {
                        case 0:
                            directionTarget = TextureTarget.TextureCubeMapNegativeX;
                            break;
                        case 1:
                            directionTarget = TextureTarget.TextureCubeMapNegativeY;
                            break;
                        case 2:
                            directionTarget = TextureTarget.TextureCubeMapNegativeZ;
                            break;
                        case 3:
                            directionTarget = TextureTarget.TextureCubeMapPositiveX;
                            break;
                        case 4:
                            directionTarget = TextureTarget.TextureCubeMapPositiveY;
                            break;
                        case 5:
                            directionTarget = TextureTarget.TextureCubeMapPositiveZ;
                            break;
                        default:
                            goto case 0;
                    }

                    GL.TexImage2D(directionTarget, i, this.PixelInternalFormat, w, h, 0, meta.PixelFormat, meta.PixelType, levelBytes);

                    // Move offse
                    srcOffset += amount;
                }

                // Calculate next size
                w = w > 1 ? w / 2 : 1;
                h = h > 1 ? h / 2 : 1;
            }

            Quality.PregeneratedMipmaps = true;
        }

    }
}
