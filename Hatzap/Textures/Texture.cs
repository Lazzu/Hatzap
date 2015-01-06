using OpenTK.Graphics.OpenGL;
using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using Hatzap.Utilities;
using System.Runtime.InteropServices;

namespace Hatzap.Textures
{
    public class Texture
    {
        public int ID { get; protected set; }
        
        public TextureTarget TextureTarget { get; protected set; }
        public PixelInternalFormat PixelInternalFormat { get; set; }

        public int Width { get; protected set; }
        public int Height { get; protected set; }

        // TODO: Populate this property automatically when texture is loaded
        public bool HasAlpha { get; set; }

        public TextureQuality Quality { get; set; }

        bool released = false;

        TextureMeta savedMeta;
        public TextureMeta Metadata
        {
            get
            {
                if (savedMeta == null)
                {
                    savedMeta = new TextureMeta()
                    {
                        FileName = "Unnamed",
                        Type = TextureType.Texture2D,
                        PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                        PixelType = PixelType.UnsignedByte,
                        Height = Height,
                        Width = Width,
                        Depth = 0,
                        PixelInternalFormat = PixelInternalFormat,
                        Quality = Quality
                    };
                }

                return savedMeta;
            }
        }

        public Texture()
        {
            ID = GL.GenTexture();
            TextureTarget = TextureTarget.Texture2D;
            PixelInternalFormat = PixelInternalFormat.Rgba;
        }

        public Texture(int width, int height)
            : this()
        {
            Width = width;
            Height = height;
        }

        public Texture(TextureMeta meta)
            : this(meta.Width, meta.Height)
        {
            // TODO: Complete member initialization
            savedMeta = meta;
            PixelInternalFormat = meta.PixelInternalFormat;
            Quality = meta.Quality;
        }

        public void Bind()
        {
            if (released)
                throw new InvalidOperationException("A texture has been released and can not be bound.");

            // Check if this texture is already bound
            Texture tmp = null;
            if (Bound.TryGetValue(TextureTarget, out tmp) && tmp == this)
            {
                return;
            }

            Time.StartTimer("Texture.Bind()", "Render");

            // Set this texture as bound texture
            Bound[TextureTarget] = this;
            GL.BindTexture(TextureTarget, ID);
            
            Time.StopTimer("Texture.Bind()");
        }

        public void UnBind()
        {
            // Check if the current TextureTarget is bound
            if (!Bound.ContainsKey(TextureTarget))
                return;

            // Set current texture as unbound
            Bound[TextureTarget] = null;
            GL.BindTexture(TextureTarget, 0);
        }

        /// <summary>
        /// Checks for quality setting changes and if needed updates the settings and generates mipmaps.
        /// </summary>
        public void UpdateQuality()
        {
            if(Quality != null && Quality.Dirty)
            {
                Quality.Update(TextureTarget);
            }
        }

        public void Generate(OpenTK.Graphics.OpenGL.PixelFormat format, PixelType type)
        {
            var meta = this.Metadata;

            meta.PixelType = type;
            meta.PixelFormat = format;

            Time.StartTimer("Texture.Generate()", "Loading");

            // Get last bound texture
            Texture last = null;
            Bound.TryGetValue(TextureTarget, out last);

            // Bind current texture
            Bind();

            // Reserve an empty image from GPU memory
            GL.TexImage2D(TextureTarget, 0, this.PixelInternalFormat, Width, Height, 0, format, type, IntPtr.Zero);

            // Restore previous state
            if (last != null) last.Bind();
            else UnBind();

            Time.StopTimer("Texture.Generate()");
        }

        public void Load(Bitmap bmp, PixelInternalFormat format)
        {
            var meta = this.Metadata;

            PixelInternalFormat = format;

            // Get last bound texture
            Texture last = null;
            Bound.TryGetValue(TextureTarget, out last);

            // Bind current texture
            Bind();

            //var transparent = HasTransparentPixels(bmp);

            BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            Width = bmp.Width;
            Height = bmp.Height;

            meta.Width = Width;
            meta.Height = Height;
            
            // Push image data to the gpu
            GL.TexImage2D(TextureTarget, 0, this.PixelInternalFormat, Width, Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bitmapData.Scan0);

            bmp.UnlockBits(bitmapData);

            // Restore previous state
            if (last != null) last.Bind();
            else UnBind();
        }

        public virtual void Load(byte[] rawBytes, TextureMeta meta)
        {
            if (meta.Type != TextureType.Texture2D)
            {
                throw new ArgumentException("Texture type must be Texture2D. Texture class does not support loading other types.");
            }

            savedMeta = meta;
            Width = meta.Width;
            Height = meta.Height;
            PixelInternalFormat = meta.PixelInternalFormat;

            int levels = 1;

            if(meta.PreMipmapped)
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

            for(int i = 0; i < levels; i++)
            {
                // How many bytes on this level?
                int amount = w * h * bpp;

                // Copy bytes to separate buffer
                var levelBytes = new byte[amount];
                Array.Copy(rawBytes, srcOffset, levelBytes, 0, amount);
                //Buffer.BlockCopy(rawBytes, srcOffset, levelBytes, 0, amount);

                // Upload bytes to the GPU

                if(meta.Precompressed)
                {
                    GL.CompressedTexImage2D(TextureTarget, i, this.PixelInternalFormat, w, h, 0, levelBytes.Length, levelBytes);
                }
                else
                {
                    GL.TexImage2D(TextureTarget, i, this.PixelInternalFormat, w, h, 0, meta.PixelFormat, meta.PixelType, levelBytes);
                }

                // Move offset and calculate next level size
                srcOffset += amount;
                w = w > 1 ? w / 2 : 1;
                h = h > 1 ? h / 2 : 1;
            }

            Quality.PregeneratedMipmaps = true;
        }

        /// <summary>
        /// Saves the bytes of a texture in to a file specified by TextureMeta. Quality settings, PixelType and PixelInternalFormat 
        /// will be changed in the TextureMeta object. If Precompressed flag is set to true in the TextureMeta object, the compressed
        /// data will be saved if the texture is compressed in memory.
        /// </summary>
        /// <param name="meta">The texture metadata object</param>
        public byte[] GetPixels(TextureMeta meta)
        {
            meta.Quality = Quality;
            meta.PixelType = PixelType.UnsignedByte;

            Time.StartTimer("Texture.GetBytes()", "GPU Data Transfer");

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
            if(meta.Precompressed)
            {
                // Find out if the texture is actually compressed in memory so we can save the compressed bytes
                int compression = 0;
                GL.GetTexLevelParameter(TextureTarget, 0, GetTextureParameter.TextureCompressed, out compression);
                if (compression == 0)
                    meta.Precompressed = false;
            }

            // If texture is not compressed, get correct amount of bytes per pixel
            if(!meta.Precompressed)
            {
                bpp = GetBpp(meta.PixelFormat);

                if(bpp == 0)
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

            for(int i = 0; i < levels; i++)
            {
                // Get the non-compressed byte array size
                var levelSize = w * h * bpp;

                // Find out the correct byte array size for compressed texture
                if (meta.Precompressed)
                {
                    GL.GetTexLevelParameter(TextureTarget, i, GetTextureParameter.TextureCompressedImageSize, out levelSize);
                }

                // Allocate the byte array
                var data = new byte[levelSize];

                // Get the texture bytes from the GPU
                if (meta.Precompressed)
                {
                    GL.GetCompressedTexImage(TextureTarget, i, data);
                }
                else
                {
                    GL.GetTexImage(TextureTarget, i, meta.PixelFormat, meta.PixelType, data);
                }

                levelBytes[i] = data;
                rawDataSize += levelSize;

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

            Time.StopTimer("Texture.GetBytes()");

            return rawdata;
        }

        public void SaveAs(string filename, ImageFormat format)
        {
            TextureMeta meta = new TextureMeta();
            meta.Quality = Quality;
            meta.PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat.Bgra;
            meta.PixelType = PixelType.UnsignedByte;
            meta.PreMipmapped = false;
            meta.Precompressed = false;

            var bytes = GetPixels(meta);

            var bmp = new Bitmap(Width, Height, Width * 4, System.Drawing.Imaging.PixelFormat.Format32bppArgb, Marshal.UnsafeAddrOfPinnedArrayElement(bytes, 0));

            bmp.Save(filename, format);
        }

        /// <summary>
        /// Uploads a region from source buffer to the texture. Currently only Bgra pixel format is supported.
        /// WARNING: Unplanned feature, non-stable API.
        /// </summary>
        /// <param name="source">A pointer to the source buffer</param>
        /// <param name="x">Region position x</param>
        /// <param name="y">Region position y</param>
        /// <param name="w">Region size width</param>
        /// <param name="h">Region size height</param>
        public void UploadRegion(IntPtr source, int x, int y, int w, int h)
        {
            Time.StartTimer("Texture.UploadRegion()", "Loading");

            // Get last bound texture
            Texture last = null;
            Bound.TryGetValue(TextureTarget, out last);

            // Bind current texture
            Bind();

            // Set texture row width
            GL.PixelStore(PixelStoreParameter.UnpackRowLength, Width);

            // Upload pixels
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, x, y, w, h, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, source);

            // Reset texture row width
            GL.PixelStore(PixelStoreParameter.UnpackRowLength, 0);

            Time.StopTimer("Texture.UploadRegion()");

            // Restore previous state
            if (last != null) last.Bind();
            else UnBind();
        }

        /// <summary>
        /// Release the texture for removal in GPU memory.
        /// </summary>
        public void Release()
        {
            GL.DeleteTexture(ID);
            ID = -1;
        }

        protected bool HasTransparentPixels(Bitmap bmp)
        {
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    var pixel = bmp.GetPixel(i, j);
                    if (pixel.A != 255)
                        return true;
                }
            }
            return false;
        }

        public static int GetBpp(OpenTK.Graphics.OpenGL.PixelFormat format)
        {
            int bpp = 0;

            switch (format)
            {
                case OpenTK.Graphics.OpenGL.PixelFormat.Bgra:
                case OpenTK.Graphics.OpenGL.PixelFormat.Rgba:
                    bpp = 4;
                    break;
                case OpenTK.Graphics.OpenGL.PixelFormat.Bgr:
                case OpenTK.Graphics.OpenGL.PixelFormat.Rgb:
                    bpp = 3;
                    break;
                case OpenTK.Graphics.OpenGL.PixelFormat.Rg:
                    bpp = 2;
                    break;
                case OpenTK.Graphics.OpenGL.PixelFormat.Red:
                case OpenTK.Graphics.OpenGL.PixelFormat.Green:
                case OpenTK.Graphics.OpenGL.PixelFormat.Blue:
                case OpenTK.Graphics.OpenGL.PixelFormat.Alpha:
                    bpp = 1;
                    break;
            }

            return bpp;
        }

        public static Dictionary<TextureTarget, Texture> Bound = new Dictionary<TextureTarget,Texture>();
        
    }
}
