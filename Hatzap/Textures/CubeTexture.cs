using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hatzap.Utilities;
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

        public override byte[] GetPixels(TextureMeta meta)
        {
            meta.Quality = Quality;
            meta.PixelType = PixelType.UnsignedByte;

            Time.StartTimer("CubeTexture.GetBytes()", "GPU Data Transfer");

            // Get last bound texture
            Texture last = null;
            Bound.TryGetValue(TextureTarget, out last);

            // Bind current texture
            Bind();

            int w = Width;  // Width of the image
            int h = Height; // Height of the image
            int bpp = 0;    // Bytes per pixel for uncompressed textures
            int levels = 1; // Mipmap levels, default = no mipmaps, just one texture level

            // Get the internal format
            int format;
            GL.GetTexLevelParameter(TextureTarget, 0, GetTextureParameter.TextureInternalFormat, out format);
            meta.PixelInternalFormat = (OpenTK.Graphics.OpenGL.PixelInternalFormat)format;

            // Check for texture compression
            if (meta.Precompressed)
            {
                // Find out if the texture is actually compressed in memory so we can save the compressed bytes
                int compression = 0;
                GL.GetTexLevelParameter(TextureTarget, 0, GetTextureParameter.TextureCompressed, out compression);
                if (compression == 0)
                    meta.Precompressed = false;
            }

            // If texture is not compressed, get correct amount of bytes per pixel
            if (!meta.Precompressed)
            {
                bpp = GetBpp(meta.PixelFormat);

                if (bpp == 0)
                    throw new NotImplementedException("The selected PixelFormat used for saving texture to file not yet supported.");
            }

            // Check if mipmaps are enabled
            if (meta.PreMipmapped)
            {
                meta.PreMipmapped = Quality.Mipmaps;
            }

            // If mipmap saving is enabled, get how many levels there are in the texture
            if (meta.PreMipmapped)
            {
                int size = Math.Max(Width, Height);
                levels += (int)Math.Floor(Math.Log(size, 2));
            }

            var levelBytes = new byte[levels][];
            int rawDataSize = 0;

            for (int i = 0; i < levels; i++)
            {
                // Allocate the byte array
                var levelData = new List<byte>();

                for (var direction = 0; direction < 6; direction++)
                {
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

                    // Get the non-compressed byte array size
                    var partSize = w * h * bpp;

                    // Find out the correct byte array size for compressed texture
                    if (meta.Precompressed)
                    {
                        GL.GetTexLevelParameter(directionTarget, i, GetTextureParameter.TextureCompressedImageSize, out partSize);
                    }

                    var data = new byte[partSize];

                    // Get the texture bytes from the GPU
                    if (meta.Precompressed)
                    {
                        GL.GetCompressedTexImage(directionTarget, i, data);
                    }
                    else
                    {
                        GL.GetTexImage(directionTarget, i, meta.PixelFormat, meta.PixelType, data);
                    }

                    levelData.AddRange(data);
                }

                levelBytes[i] = levelData.ToArray();
                rawDataSize += levelData.Count;

                // Calculate next level size
                w = w > 1 ? w / 2 : 1;
                h = h > 1 ? h / 2 : 1;
            }

            var rawdata = new byte[rawDataSize];
            int offset = 0;

            for (int i = 0; i < levels; i++)
            {
                int blocksize = levelBytes[i].Length;
                Array.Copy(levelBytes[i], 0, rawdata, offset, blocksize);
                offset += blocksize;
            }

            // Restore previous state
            if (last != null) last.Bind();
            else UnBind();

            Time.StopTimer("CubeTexture.GetBytes()");

            return rawdata;
        }

        public override void SaveAs(string filename, System.Drawing.Imaging.ImageFormat format)
        {
            throw new NotImplementedException();
        }

    }
}
