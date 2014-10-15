using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hatzap.Rendering;

namespace Hatzap.Models
{
    public class Material : IList<IUniformData>, IEnumerable<IUniformData>
    {
        public List<IUniformData> UniformData { get; set; }

        public IEnumerable<IUniformData> Uniforms 
        { 
            get
            {
                for (int i = 0; i < UniformData.Count; i++)
                {
                    if(UniformData[i] != null) yield return UniformData[i];
                }
            }
        }

        public Material()
        {
            UniformData = new List<IUniformData>();
        }

        public int IndexOf(IUniformData item)
        {
            return UniformData.IndexOf(item);
        }

        public void Insert(int index, IUniformData item)
        {
            UniformData.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            UniformData.RemoveAt(index);
        }

        public IUniformData this[int index]
        {
            get
            {
                return UniformData[index];
            }
            set
            {
                if(value == null)
                {
                    RemoveAt(index);
                }
                else
                {
                    UniformData[index] = value;
                }
            }
        }

        public void Add(IUniformData item)
        {
            UniformData.Add(item);
        }

        public void Clear()
        {
            UniformData.Clear();
        }

        public bool Contains(IUniformData item)
        {
            return UniformData.Contains(item);
        }

        public void CopyTo(IUniformData[] array, int arrayIndex)
        {
            UniformData.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return UniformData.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(IUniformData item)
        {
            return UniformData.Remove(item);
        }

        public IEnumerator<IUniformData> GetEnumerator()
        {
            return UniformData.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return UniformData.GetEnumerator();
        }

        IEnumerator<IUniformData> IEnumerable<IUniformData>.GetEnumerator()
        {
            return UniformData.GetEnumerator();
        }

        public Material Copy()
        {
            Material m = new Material();
            foreach (var data in UniformData)
            {
                throw new NotImplementedException();
            }
            return m;
        }
    }
}
