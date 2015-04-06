using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using Hatzap.Utilities;
using System.Diagnostics;

namespace Hatzap.Textures
{
    [Serializable]
    public class TextureQuality
    {
        #region private fields

        private TextureFiltering textureFiltering;
        [NonSerialized]
        private bool textureFilteringDirty = true;

        private float anisotrophy;
        [NonSerialized]
        private bool anisotrophyDirty = true;

        private TextureWrapMode textureWrapMode_S;
        [NonSerialized]
        private bool textureWrapMode_SDirty = true;

        private TextureWrapMode textureWrapMode_T;
        [NonSerialized]
        private bool textureWrapMode_TDirty = true;

        private bool mipmaps;
        [NonSerialized]
        private bool mipmapsDirty = true;

        private bool pregeneratedMipmaps = false;
        private int mipmapLevels = -1;

        #endregion

        #region public properties

        public TextureFiltering Filtering {
            get
            {
                return textureFiltering;
            }
            set
            {
                if (textureFiltering != value)
                {
                    textureFiltering = value;
                    textureFilteringDirty = true;
                }
            }
        }
        public float Anisotrophy
        {
            get
            {
                return anisotrophy;
            }
            set
            {
                if (anisotrophy != value)
                {
                    anisotrophy = value;

                    if (anisotrophy < 0.0f)
                        anisotrophy = 0;

                    anisotrophyDirty = true;
                }
            }
        }
        public TextureWrapMode TextureWrapMode_S
        {
            get
            {
                return textureWrapMode_S;
            }
            set
            {
                if (textureWrapMode_S != value)
                {
                    textureWrapMode_S = value;
                    textureWrapMode_SDirty = true;
                }
            }
        }
        public TextureWrapMode TextureWrapMode_T
        {
            get
            {
                return textureWrapMode_T;
            }
            set
            {
                if (textureWrapMode_T != value)
                {
                    textureWrapMode_T = value;
                    textureWrapMode_TDirty = true;
                }
            }
        }

        public TextureWrapMode TextureWrapMode
        {
            set
            {
                TextureWrapMode_S = value;
                TextureWrapMode_T = value;
            }
        }
        
        public bool Mipmaps { 
            get
            {
                return mipmaps;
            }
            set
            {
                if (mipmaps != value)
                {
                    mipmaps = value;
                    mipmapsDirty = true;
                    textureFilteringDirty = true;
                }
            }
        }

        public int MipmapLevels
        {
            get
            {
                return mipmapLevels;
            }
            set
            {
                if (mipmapLevels != value)
                {
                    mipmapLevels = value;
                    mipmapsDirty = true;
                    if (!pregeneratedMipmaps)
                    {
                        textureFilteringDirty = true;
                    }
                }
            }
        }

        public bool PregeneratedMipmaps
        {
            get
            {
                return pregeneratedMipmaps;
            }
            set
            {
                if (pregeneratedMipmaps != value)
                {
                    pregeneratedMipmaps = value;
                    if (!pregeneratedMipmaps)
                    {
                        mipmapsDirty = true;
                        textureFilteringDirty = true;
                    }
                }
            }
        }

        public bool Dirty
        {
            get
            {
                return textureFilteringDirty || 
                    anisotrophyDirty || 
                    textureWrapMode_SDirty || 
                    textureWrapMode_TDirty;
            }
        }

        #endregion

        public TextureQuality()
        {
            Filtering = TextureFiltering.Bilinear;
            TextureWrapMode_S = TextureWrapMode.Repeat;
            TextureWrapMode_T = TextureWrapMode.Repeat;
            Anisotrophy = 1;
            mipmaps = false;
        }

        /// <summary>
        /// Update the texture quality settings for the currently bound texture
        /// </summary>
        /// <param name="target">The GL texture target</param>
        public void Update(TextureTarget target)
        {
            if(mipmapsDirty)
            {
                mipmapsDirty = false;

                if(mipmaps)
                {
                    GL.TexParameter(target, TextureParameterName.TextureBaseLevel, 0);

                    int levels = mipmapLevels;
                    if (levels < 0)
                    {
                        levels = 1000;
                    }
                    //GL.TexParameter(target, TextureParameterName.TextureMaxLevel, levels);

                    if(!pregeneratedMipmaps)
                    {
                        GL.TexParameter(target, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                        GL.TexParameter(target, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);                    

                        switch (target)
                        {
                            case TextureTarget.Texture1D:
                                GL.GenerateMipmap(GenerateMipmapTarget.Texture1D);
                                break;
                            case TextureTarget.Texture2D:
                                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
                                break;
                            case TextureTarget.Texture3D:
                                GL.GenerateMipmap(GenerateMipmapTarget.Texture3D);
                                break;
                            case TextureTarget.Texture1DArray:
                                GL.GenerateMipmap(GenerateMipmapTarget.Texture1DArray);
                                break;
                            case TextureTarget.Texture2DArray:
                                GL.GenerateMipmap(GenerateMipmapTarget.Texture2DArray);
                                break;
                            case TextureTarget.TextureCubeMap:
                                GL.GenerateMipmap(GenerateMipmapTarget.TextureCubeMap);
                                break;
                            case TextureTarget.TextureCubeMapArray:
                                GL.GenerateMipmap(GenerateMipmapTarget.TextureCubeMapArray);
                                break;
                            case TextureTarget.Texture2DMultisample:
                                GL.GenerateMipmap(GenerateMipmapTarget.Texture2DMultisample);
                                break;
                            case TextureTarget.Texture2DMultisampleArray:
                                GL.GenerateMipmap(GenerateMipmapTarget.Texture2DMultisampleArray);
                                break;
                            default:
                                throw new NotImplementedException("Generating mipmaps not supported for TextureTarget." + target.ToString() + ". Currently supported mipmaps are: Texture1D, Texture2D, Texture3D, Texture1DArray, Texture2DArray, TextureCubeMap, TextureCubeMapArray, Texture2DMultisample, Texture2DMultisampleArray.");
                        }
                    }
                }
            }

            if(textureFilteringDirty)
            {

                if(target == TextureTarget.Texture2D || target == TextureTarget.TextureCubeMap)
                {
                    switch (textureFiltering)
                    {
                        case TextureFiltering.Nearest:

                            GL.TexParameter(target, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

                            if (mipmaps)
                            {
                                GL.TexParameter(target, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.NearestMipmapNearest);
                            }
                            else
                            {
                                GL.TexParameter(target, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                            }

                            break;

                        case TextureFiltering.Bilinear:

                            GL.TexParameter(target, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

                            if (mipmaps)
                            {
                                GL.TexParameter(target, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapNearest);
                            }
                            else
                            {
                                GL.TexParameter(target, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                            }

                            break;

                        case TextureFiltering.Trilinear:

                            GL.TexParameter(target, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

                            if (mipmaps)
                            {
                                GL.TexParameter(target, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
                            }
                            else
                            {
                                GL.TexParameter(target, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                            }

                            break;
                    }
                }

                textureFilteringDirty = false;
            }
            
            if (anisotrophyDirty)
            {
                if (GPUCapabilities.AnisotrophicFiltering && (target == TextureTarget.TextureCubeMap || target == TextureTarget.Texture2D))
                {
                    if (GPUCapabilities.MaxAnisotrophyLevel < anisotrophy)
                        anisotrophy = GPUCapabilities.MaxAnisotrophyLevel;

                    if (anisotrophy < 1)
                        anisotrophy = 1;

                    GL.TexParameter(target, (TextureParameterName)ExtTextureFilterAnisotropic.TextureMaxAnisotropyExt, anisotrophy);
                }
                
                anisotrophyDirty = false;
            }

            if(textureWrapMode_SDirty)
            {
                if (target == TextureTarget.TextureCubeMap || target == TextureTarget.Texture2D)
                {
                    GL.TexParameter(target, TextureParameterName.TextureWrapS, (int)textureWrapMode_S);
                }
                textureWrapMode_SDirty = false;
            }

            if (textureWrapMode_TDirty)
            {
                if (target == TextureTarget.TextureCubeMap || target == TextureTarget.Texture2D)
                {
                    GL.TexParameter(target, TextureParameterName.TextureWrapT, (int)textureWrapMode_T);
                }
                
                textureWrapMode_TDirty = false;
            }
        }
    }
}
