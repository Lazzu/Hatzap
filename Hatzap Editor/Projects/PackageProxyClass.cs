using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hatzap.Assets;

namespace Hatzap_Editor.Projects
{
    public class PackageProxyClass : ICollection<IPackageItem>
    {
        public void Add(IPackageItem item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(IPackageItem item)
        {
            return PackageManager.AssetExists(item.Path);
        }

        public void CopyTo(IPackageItem[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public bool Remove(IPackageItem item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<IPackageItem> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
