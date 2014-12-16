using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public TextureQuality Quality { get { return quality; } set { quality = value; qualityDirty = true; } }

        TextureQuality quality;
        private bool qualityDirty;

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
            Time.StartTimer("Overhead", "Overhead");

            // Check if this texture is already bound
            Texture tmp = null;
            if (Bound.TryGetValue(TextureTarget, out tmp) && tmp == this)
            {
                Time.StopTimer("Overhead");
                return;
            }

            Time.StopTimer("Overhead");
            Time.StartTimer("Texture.Bind()", "Render");

            // Set this texture as bound texture
            Bound[TextureTarget] = this;
            GL.BindTexture(TextureTarget, ID);

            if (qualityDirty && quality != null)
                SetQuality();

            Time.StopTimer("Texture.Bind()");
        }

        public void UnBind()
        {
            // Check if there are any textures bound
            if (!Bound.ContainsKey(TextureTarget) || Bound[TextureTarget] == null)
                return;

            // Set current texture as unbound
            Bound[TextureTarget] = null;
            GL.BindTexture(TextureTarget, 0);
        }

        public void TextureSettings(TextureMinFilter minFilter, TextureMagFilter magFilter, float anisotrophy)
        {
            TextureSettings(new TextureQuality()
            {
                TextureMinFilter = minFilter,
                TextureMagFilter = magFilter,
                Anisotrophy = anisotrophy
            });
        }

        public void TextureSettings(TextureQuality quality)
        {
            if (quality.Anisotrophy < 0.0f)
                throw new GraphicsException("Texture anistrophy setting can not be less than zero.");

            Quality = quality;
        }

        void SetQuality()
        {
            qualityDirty = false;

            // Set texture filtering options
            GL.TexParameter(TextureTarget, TextureParameterName.TextureMinFilter, (int)quality.TextureMinFilter);
            GL.TexParameter(TextureTarget, TextureParameterName.TextureMagFilter, (int)quality.TextureMagFilter);

            float maxAniso = 0.0f;
            GL.GetFloat((GetPName)ExtTextureFilterAnisotropic.MaxTextureMaxAnisotropyExt, out maxAniso);

            if (maxAniso > quality.Anisotrophy)
            {
                maxAniso = quality.Anisotrophy;
            }

            if (maxAniso > 0)
                GL.TexParameter(TextureTarget, (TextureParameterName)ExtTextureFilterAnisotropic.TextureMaxAnisotropyExt, maxAniso);

            // Wrap options
            GL.TexParameter(TextureTarget, TextureParameterName.TextureWrapS, (int)quality.TextureWrapMode_S);
            GL.TexParameter(TextureTarget, TextureParameterName.TextureWrapT, (int)quality.TextureWrapMode_T);
        }

        public void Generate(OpenTK.Graphics.OpenGL.PixelFormat format, PixelType type)
        {
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

        public void Generate(OpenTK.Graphics.OpenGL.PixelFormat format, PixelType type, TextureMinFilter minFilter, TextureMagFilter magFilter, float anisotrophy)
        {
            Time.StartTimer("Texture.Generate()", "Loading");

            // Get last bound texture
            Texture last = null;
            Bound.TryGetValue(TextureTarget, out last);

            // Bind current texture
            Bind();

            // Reserve an empty image from GPU memory
            GL.TexImage2D(TextureTarget, 0, this.PixelInternalFormat, Width, Height, 0, format, type, IntPtr.Zero);

            TextureSettings(minFilter, magFilter, anisotrophy);

            // Restore previous state
            if (last != null) last.Bind();
            else UnBind();

            Time.StopTimer("Texture.Generate()");
        }

        public void Load(Bitmap bmp, OpenTK.Graphics.OpenGL.PixelFormat format, PixelType type, TextureMinFilter minFilter, TextureMagFilter magFilter, float anisotrophy, bool mipmaps)
        {
            Time.StartTimer("Texture.Load()", "Loading");

            

            // Get last bound texture
            Texture last = null;
            Bound.TryGetValue(TextureTarget, out last);

            // Bind current texture
            Bind();

            var transparent = HasTransparentPixels(bmp);

            BitmapData bitmapData = bmp.LockBits(new Rectangle(0,0,bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            Width = bmp.Width;
            Height = bmp.Height;

            // Reserve an empty image from GPU memory
            GL.TexImage2D(TextureTarget, 0, this.PixelInternalFormat, Width, Height, 0, format, type, bitmapData.Scan0);

            bmp.UnlockBits(bitmapData);

            if (mipmaps)
            {
                GenMipMaps();
            }

            TextureSettings(minFilter, magFilter, anisotrophy);

            // Restore previous state
            if (last != null) last.Bind();
            else UnBind();

            Time.StopTimer("Texture.Load()");
        }

        public void Load(Bitmap bmp, OpenTK.Graphics.OpenGL.PixelFormat format, PixelType type)
        {
            Time.StartTimer("Texture.Load()", "Loading");

            // Get last bound texture
            Texture last = null;
            Bound.TryGetValue(TextureTarget, out last);

            // Bind current texture
            Bind();

            var transparent = HasTransparentPixels(bmp);

            BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            Width = bmp.Width;
            Height = bmp.Height;

            // Reserve an empty image from GPU memory
            GL.TexImage2D(TextureTarget, 0, this.PixelInternalFormat, Width, Height, 0, format, type, bitmapData.Scan0);

            bmp.UnlockBits(bitmapData);

            // Restore previous state
            if (last != null) last.Bind();
            else UnBind();

            Time.StopTimer("Texture.Load()");
        }

        public void Load(TextureMeta meta)
        {
            Load(new Bitmap(meta.FileName), meta.PixelFormat, meta.PixelType);
            Bind();
            if (meta.Mipmaps)
                GenMipMaps();
            TextureSettings(meta.TextureMinFilter, meta.TextureMagFilter, meta.AnisotrophicFiltering);
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

        public void GenMipMaps()
        {
            GL.TexParameter(TextureTarget, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
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
