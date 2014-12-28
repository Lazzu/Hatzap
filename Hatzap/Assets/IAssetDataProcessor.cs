using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hatzap.Assets
{
    public interface IAssetDataProcessor
    {
        Stream AssetRead(Stream source);
        Stream PackagaeRead(Stream source);
        Stream AssetWrite(Stream source);
        Stream PackagaeWrite(Stream source);
    }
}
