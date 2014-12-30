using OpenTK.Graphics.OpenGL;
using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using Hatzap.Utilities;

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
                        PixelInternalFormat = PixelInternalFormat,
                        Quality = Quality
                    };
                }

                return savedMeta.Copy;
            }
        }

        public Texture()
        {
            ID = GL.GenTexture();
            TextureTarget = TextureTarget.Texture2D;
            PixelInternalFormat = PixelInternalFormat.Rgba;
        }

        public Texture(int width, int height) : this()
        {
            Width = width;
            Height = height;
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

        void Load(Bitmap bmp, OpenTK.Graphics.OpenGL.PixelFormat format, PixelType type)
        {
            var meta = this.Metadata;

            meta.PixelType = type;
            meta.PixelFormat = format;

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
            GL.TexImage2D(TextureTarget, 0, this.PixelInternalFormat, Width, Height, 0, format, type, bitmapData.Scan0);

            bmp.UnlockBits(bitmapData);

            // Restore previous state
            if (last != null) last.Bind();
            else UnBind();
        }

        /// <summary>
        /// Loads compressed data in to the texture. Untested code.
        /// </summary>
        /// <param name="data">Byte array containing the bytes of the texture</param>
        /// <param name="width">Width of the texture</param>
        /// <param name="height">Height of the texture</param>
        void LoadCompressed(byte[] data, int width, int height)
        {
            // Get last bound texture
            Texture last = null;
            Bound.TryGetValue(TextureTarget, out last);

            // Bind current texture
            Bind();

            Width = width;
            Height = height;
            
            // Push image data to the gpu
            GL.CompressedTexImage2D(TextureTarget, 0, this.PixelInternalFormat, Width, Height, 0, data.Length, data);
            
            // Restore previous state
            if (last != null) last.Bind();
            else UnBind();
        }

        /// <summary>
        /// Load a texture to the GPU memory.
        /// </summary>
        /// <param name="meta">Texture metadata</param>
        public void Load(TextureMeta meta)
        {
            savedMeta = meta;

            Quality = meta.Quality;

            PixelInternalFormat = meta.PixelInternalFormat;

            if(meta.Precompressed)
            {
                Time.StartTimer("Texture.LoadCompressed()", "Loading");

                var bytes = File.ReadAllBytes(meta.FileName);
                LoadCompressed(bytes, meta.Width, meta.Height);

                Time.StopTimer("Texture.LoadCompressed()");
            }
            else
            {
                Time.StartTimer("Texture.Load()", "Loading");

                var path = Path.GetFullPath(meta.FileName);

                using (var bmp = new Bitmap(path))
                {
                    Load(bmp, meta.PixelFormat, meta.PixelType);
                }

                Time.StopTimer("Texture.Load()");
            }

            //UpdateQuality();
        }

        /// <summary>
        /// Saves the bytes of a texture in to a file specified by TextureMeta. Quality settings, PixelType and PixelInternalFormat 
        /// will be changed in the TextureMeta object. If Precompressed flag is set to true in the TextureMeta object, the compressed
        /// data will be saved if the texture is compressed in memory.
        /// </summary>
        /// <param name="meta">The texture metadata object</param>
        public void Save(TextureMeta meta)
        {
            meta.Quality = Quality;
            meta.PixelType = PixelType.UnsignedByte;

            Time.StartTimer("Texture.Save()", "Disk Write");

            // Get last bound texture
            Texture last = null;
            Bound.TryGetValue(TextureTarget, out last);

            // Bind current texture
            Bind();

            var dataSize = Width * Height;

            // Find out if the texture is compressed in the memory
            int compression;
            GL.GetTexLevelParameter(TextureTarget, 0, GetTextureParameter.TextureCompressed, out compression);

            // Find out the correct byte array size for each type of different textures
            if(meta.Precompressed && compression == 1)
            {
                GL.GetTexLevelParameter(TextureTarget, 0, GetTextureParameter.TextureCompressedImageSize, out dataSize);
                int format;
                GL.GetTexLevelParameter(TextureTarget, 0, GetTextureParameter.TextureInternalFormat, out format);
                meta.PixelInternalFormat = (OpenTK.Graphics.OpenGL.PixelInternalFormat)format;
            }
            else
            {
                switch (meta.PixelFormat)
                {
                    case OpenTK.Graphics.OpenGL.PixelFormat.Bgra:
                    case OpenTK.Graphics.OpenGL.PixelFormat.Rgba:
                        dataSize *= 4;
                        break;
                    case OpenTK.Graphics.OpenGL.PixelFormat.Bgr:
                    case OpenTK.Graphics.OpenGL.PixelFormat.Rgb:
                        dataSize *= 3;
                        break;
                    case OpenTK.Graphics.OpenGL.PixelFormat.Rg:
                        dataSize *= 2;
                        break;
                    case OpenTK.Graphics.OpenGL.PixelFormat.Red:
                    case OpenTK.Graphics.OpenGL.PixelFormat.Green:
                    case OpenTK.Graphics.OpenGL.PixelFormat.Blue:
                        break;
                    default:
                        throw new NotImplementedException("The selected PixelFormat used for saving texture to file not yet supported.");
                }
            }

            // Allocate the data array
            var data = new byte[dataSize];

            // Get the texture bytes from the GPU
            if(meta.Precompressed)
            {
                GL.GetCompressedTexImage(TextureTarget, 0, data);
            }
            else
            {
                GL.GetTexImage(TextureTarget, 0, meta.PixelFormat, meta.PixelType, data);
            }

            File.WriteAllBytes(meta.FileName, data);

            // Restore previous state
            if (last != null) last.Bind();
            else UnBind();

            Time.StopTimer("Texture.Save()");
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

        public static Dictionary<TextureTarget, Texture> Bound = new Dictionary<TextureTarget,Texture>();
        
    }
}
