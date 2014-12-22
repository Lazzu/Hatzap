using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hatzap.Textures
{
    public enum TextureFiltering
    {
        /// <summary>
        /// Mag / Min : GL_NEAREST / GL_NEAREST, GL_NEAREST_MIPMAP_NEAREST
        /// </summary>
        Nearest,
        /// <summary>
        /// Mag / Min : GL_LINEAR / GL_LINEAR, GL_LINEAR_MIPMAP_NEAREST
        /// </summary>
        Bilinear,
        /// <summary>
        /// Mag / Min : GL_LINEAR / GL_LINEAR, GL_LINEAR_MIPMAP_LINEAR
        /// Note: For trilinear filtering to have any effect, mipmaps must be enabled. If mipmaps are disabled
        /// behaviour is the same as with bilinear filtering.
        /// </summary>
        Trilinear,

    }
}
