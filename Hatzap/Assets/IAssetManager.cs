using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hatzap.Assets
{
    public interface IAssetManager
    {
        /// <summary>
        /// Release all resources allocated by assets loaded with this manager.
        /// </summary>
        void ReleaseAll();
    }
}
