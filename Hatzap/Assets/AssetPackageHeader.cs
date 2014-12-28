using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hatzap.Assets
{
    public class AssetPackageHeader
    {
        public List<AssetMeta> Assets { get; set; }

        public string PackagePath { get; set; }

        public void Read(Stream package)
        {
            Assets = new List<AssetMeta>();

            using(BinaryReader br = new BinaryReader(package, Encoding.UTF8, true))
            {
                var count = br.ReadInt32();

                for(int i = 0; i < count; i++)
                {
                    // Read and add asset
                    Assets.Add(new AssetMeta()
                    {
                        Path = br.ReadString(),
                        Position = br.ReadInt64(),
                        Size = br.ReadInt32()
                    });
                }
            }
        }

        public void Write(Stream package)
        {
            using (BinaryWriter bw = new BinaryWriter(package, Encoding.UTF8, true))
            {
                bw.Write(Assets.Count);

                foreach (var item in Assets)
                {
                    bw.Write(item.Path);
                    bw.Write(item.Position);
                    bw.Write(item.Size);
                }
            }
        }
    }
}
