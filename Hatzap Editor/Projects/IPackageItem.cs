using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hatzap_Editor.Projects
{
    public interface IPackageItem
    {
        /// <summary>
        /// The name of the package item. For example asset.ext
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Path inside the package. For example /Assets/asset.ext
        /// </summary>
        string Path { get; set; }

        /// <summary>
        /// Path to the source file. For example /AssetSRC/asset.ext
        /// </summary>
        string Source { get; set; }
    }
}
