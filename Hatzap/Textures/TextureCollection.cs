using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hatzap.Assets;

namespace Hatzap.Textures
{
    public class TextureCollection : IAssetCollection
    {
        public List<TextureMeta> Textures { get; set; }
    }
}
