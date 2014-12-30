using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hatzap.Assets
{
    /// <summary>
    /// Asset compression / decompression. Does not support package compression.
    /// </summary>
    public class AssetDataCompressionProcessor : IAssetDataProcessor
    {
        public CompressionLevel CompressionLevel { get; set; }

        public AssetDataCompressionProcessor() : this(CompressionLevel.Optimal) { }

        public AssetDataCompressionProcessor(CompressionLevel level)
        {
            CompressionLevel = level;
        }

        public Stream AssetRead(Stream source)
        {
            return new GZipStream(source, CompressionMode.Decompress);
        }
        public Stream PackagaeRead(Stream source)
        {
            return source;
        }

        public Stream AssetWrite(Stream source)
        {
            return new GZipStream(source, CompressionLevel);
        }

        public Stream PackagaeWrite(Stream source)
        {
            return source;
        }
    }
}
