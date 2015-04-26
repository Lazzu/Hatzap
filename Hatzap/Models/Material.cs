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
        List<IUniformData> uniforms = new List<IUniformData>();
        Dictionary<string, IUniformData> uniformDictionary = new Dictionary<string, IUniformData>();
        Dictionary<string, int> uniformIndex = new Dictionary<string, int>();

        public List<IUniformData> UniformData {
            get
            {
                return uniforms;
            }
            set
            {
                uniforms = value;
                uniformIndex.Clear();
                uniformDictionary.Clear();
                for (int i = 0; i < uniforms.Count; i++)
                {
                    var item = uniforms[i];
                    uniformDictionary.Add(item.Name, item);
                    uniformIndex.Add(item.Name, i);
                }
            }
        }

        public bool Transparent { get; set; }

        public int DrawingOrder { get; set; }

        public IEnumerable<IUniformData> Uniforms 
        { 
            get
            {
                for (int i = 0; i < uniforms.Count; i++)
                {
                    if (uniforms[i] != null) yield return uniforms[i];
                }
            }
        }

        public int IndexOf(IUniformData item)
        {
            return uniforms.IndexOf(item);
        }

        public void Insert(int index, IUniformData item)
        {
            uniforms.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            uniforms.RemoveAt(index);
        }

        public IUniformData this[int index]
        {
            get
            {
                return uniforms[index];
            }
            set
            {
                if(value == null)
                {
                    RemoveAt(index);
                }
                else
                {
                    uniforms[index] = value;
                }
            }
        }

        public IUniformData this[string name]
        {
            get
            {
                return uniformDictionary[name];
            }
            set
            {
                if (value == null)
                {
                    Remove(name);
                }
                else
                {
                    int index;
                    if (uniformIndex.TryGetValue(name, out index))
                    {
                        uniforms[index] = value;
                        uniformDictionary[name] = value;
                    }
                }
            }
        }

        private void Remove(string name)
        {
            int index;
            if (uniformIndex.TryGetValue(name, out index))
            {
                uniforms.RemoveAt(index);
                uniformDictionary.Remove(name);
                uniformIndex.Remove(name);
            }
        }

        public void Add(IUniformData item)
        {
            uniformIndex.Add(item.Name, uniforms.Count);
            uniforms.Add(item);
            uniformDictionary.Add(item.Name, item);
        }

        public void Clear()
        {
            uniforms.Clear();
            uniformIndex.Clear();
            uniformDictionary.Clear();
        }

        public bool Contains(IUniformData item)
        {
            return uniformIndex.ContainsKey(item.Name);
        }

        public void CopyTo(IUniformData[] array, int arrayIndex)
        {
            uniforms.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return uniforms.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(IUniformData item)
        {
            if (uniforms.Remove(item))
            {
                uniformDictionary.Remove(item.Name);
                uniformIndex.Remove(item.Name);
                return true;
            }
            return false;
        }

        public IEnumerator<IUniformData> GetEnumerator()
        {
            return uniforms.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return uniforms.GetEnumerator();
        }

        IEnumerator<IUniformData> IEnumerable<IUniformData>.GetEnumerator()
        {
            return uniforms.GetEnumerator();
        }

        public Material Copy()
        {
            Material m = new Material()
            {
                uniforms = new List<IUniformData>()
            };

            foreach (var data in uniforms)
            {
                m.Add(data.Copy());
            }
            return m;
        }
    }
}
