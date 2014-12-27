using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace Hatzap.Utilities
{
    /// <summary>
    /// OpenGL State manager
    /// </summary>
    public static class GLState
    {
        static bool depthTest = false;

        /// <summary>
        /// Enabled / Disables depth testing.
        /// </summary>
        public static bool DepthTest
        {
            get
            {
                return depthTest;
            }
            set
            {
                if (depthTest == value)
                    return;

                depthTest = value;

                if(depthTest)
                { 
                    GL.Enable(EnableCap.DepthTest);
                }
                else
                {
                    GL.Disable(EnableCap.DepthTest);
                }
            }
        }

        static bool alphaBlend = false;

        /// <summary>
        /// Enabled / Disables depth testing.
        /// </summary>
        public static bool AlphaBleding
        {
            get
            {
                return alphaBlend;
            }
            set
            {
                if (alphaBlend == value)
                    return;

                alphaBlend = value;

                if (alphaBlend) 
                { 
                    GL.Enable(EnableCap.Blend);
                }
                else
                {
                    GL.Disable(EnableCap.Blend);
                }
            }
        }

        // Picked something that should not be enabled as the first call, but still possible. This might cause bugs.
        static BlendingFactorDest alphaDst = BlendingFactorDest.Zero;
        static BlendingFactorSrc alphaSrc = BlendingFactorSrc.Zero;

        /// <summary>
        /// Set the OpenGL BlendFunc state
        /// </summary>
        /// <param name="src">BlendingFactorSrc</param>
        /// <param name="dst">BlendingFactorDest</param>
        public static void BlendFunc(BlendingFactorSrc src, BlendingFactorDest dst)
        {
            if (!(alphaDst != dst || alphaSrc != src))
                return;
            
            GL.BlendFunc(src, dst);
            alphaSrc = src;
            alphaDst = dst;
        }

        static bool cullFace = false;

        /// <summary>
        /// Enable / Disable face culling
        /// </summary>
        public static bool CullFace
        {
            get
            {
                return cullFace;
            }
            set
            {
                if (cullFace == value)
                    return;

                cullFace = value;

                if(cullFace)
                {
                    GL.Enable(EnableCap.CullFace);
                }
                else
                {
                    GL.Disable(EnableCap.CullFace);
                }
            }
        }

        static bool primitiveRestart = false;
        public static bool PrimitiveRestart {
            get
            {
                return primitiveRestart;
            }
            set
            {
                if (primitiveRestart == value)
                    return;

                primitiveRestart = value;

                if(primitiveRestart)
                {
                    GL.Enable(EnableCap.PrimitiveRestart);
                }
                else
                {
                    GL.Disable(EnableCap.PrimitiveRestart);
                }
            }
        }

        static int primitiveRestartIndex = -1;
        public static int PrimitiveRestartIndex
        {
            get
            {
                return primitiveRestartIndex;
            }
            set
            {
                if (primitiveRestartIndex == value)
                    return;

                primitiveRestartIndex = value;

                GL.PrimitiveRestartIndex(primitiveRestartIndex);
            }
        }
    }
}
