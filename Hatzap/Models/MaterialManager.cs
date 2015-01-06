using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hatzap.Assets;

namespace Hatzap.Models
{
    public class MaterialManager : AssetManagerBase<Material>
    {
        public override Material Get(string path, bool autoload = false)
        {
            var material = base.Get(path, autoload);

            if(material == null)
                return null;

            return material.Copy();
        }

        protected override Material LoadAsset(System.IO.Stream stream)
        {
            throw new NotImplementedException();
        }

        protected override void SaveAsset(Material asset, System.IO.Stream stream)
        {
            throw new NotImplementedException();
        }
    }
}
