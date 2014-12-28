using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hatzap.Assets
{
    public class AssetMeta
    {
        /// <summary>
        /// The file unique path
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The package the file belongs in
        /// </summary>
        public string Package { get; set; }

        /// <summary>
        /// The file size
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// The file position in package
        /// </summary>
        public long Position { get; set; }
    }
}
