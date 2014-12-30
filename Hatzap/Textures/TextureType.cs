using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hatzap.Textures
{
    public enum TextureType
    {
        Texture2D, // First, as it should be the default
        Texture1D,
        Texture3D,
        TextureCubemap,
        TextureArray,
        TextureArray3D,
        TextureCubemapArray,
        TextureBuffer,
        TextureRectangle,
        Texture2DMultisample,
        Texture2DMultisampleArray
    }
}
