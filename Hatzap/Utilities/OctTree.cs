using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Hatzap.Models;
using Hatzap.Rendering;
using OpenTK;

namespace Hatzap.Utilities
{
    public class OctTree<T> where T : ITransformable
    {
        public BoundingBox Box;

        public OctTree<T>[] Nodes { get; protected set; }

        public OctTree<T> Parent { get; internal set; }

        public List<T> Items { get; protected set; }

        public int Capacity { get; protected set; }

        public int Depth { get; protected set; }

        private ContainmentType result;

        public OctTree(int capacity, int depth)
        {
            Capacity = capacity;
            Depth = depth;
            Items = new List<T>();
        }

        public OctTree(int capacity, int depth, List<T> items, BoundingBox box)
        {
            Capacity = capacity;
            Depth = depth;
            Items = items;
            Box = box;
        }

        private void Split()
        {
            if (Depth == 0)
                return;

            Nodes = new OctTree<T>[8];

            Vector3 size = (Box.Max - Box.Min) / 2;

            for(int x = 0; x < 2; x++)
            for(int y = 0; y < 2; y++)
            for(int z = 0; z < 2; z++)
            {
                int index = x * 4 + y * 2 + z;

                BoundingBox box = new BoundingBox(Box.Min + size * new Vector3(x, y, z), Box.Min + size + size * new Vector3(x, y, z));
                
                List<T> list = new List<T>(GetItems(i => box.Contains(i.Transform.Position) != ContainmentType.Disjoint));
                
                Nodes[index] = new OctTree<T>(Capacity, Depth - 1, list, box);
            }

            Items = null;
        }

        public bool Insert(T item)
        {
            if(Items != null)
            {
                Items.Add(item);
                
                if (Depth <= 0 || Items.Count < Capacity)
                    return true;

                Split();

                return true;
            }
            else
            {
                for (int i = 0; i < 8; i++)
                {
                    if (Nodes[i].Contains(ref item.Transform.Position))
                    {
                        return Nodes[i].Insert(item);
                    }
                }
            }
            return false;
        }

        public void Remove(T item)
        {
            if (Items != null)
            {
                Items.Remove(item);
                return;
            }

            for (int i = 0; i < 8; i++)
            {
                if(Nodes[i].Contains(ref item.Transform.Position))
                {
                    Nodes[i].Remove(item);
                    break;
                }
            }
        }

        public bool Contains(ref Vector3 point)
        {
            Box.Contains(ref point, out result);
            return result != ContainmentType.Disjoint;
        }

        public IEnumerable<T> GetItems(BoundingFrustum frustum, bool cullPerItem = true)
        {
            if (Items == null)
            {
                for (int i = 0; i < 8; i++)
                {
                    frustum.Contains(ref Nodes[i].Box, out result);

                    if (result != ContainmentType.Disjoint)
                    {
                        // TODO: Make use of some custom iterators here to improve performance instead of using foreach
                        foreach (var item in Nodes[i].GetItems(frustum, cullPerItem))
                        {
                            yield return item;
                        }
                    }
                }

                yield break;
            }
            else
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    if(!cullPerItem || frustum.Contains(Items[i].Transform.Position) != ContainmentType.Disjoint)
                    yield return Items[i];
                }
            }
        }

        public IEnumerable<T> GetItems(Func<T, bool> comparer)
        {
            if (Items == null)
            {
                for (int i = 0; i < 8; i++)
                {
                    foreach (var item in Nodes[i].GetItems(comparer))
                    {
                        yield return item;
                    }
                }

                yield break;
            }
            else
            {
                //Debug.WriteLine("Octree.GetItems(comparer) Item count: " + Items.Count);
                for (int i = 0; i < Items.Count; i++)
                {
                    if(comparer(Items[i])) yield return Items[i];
                }
            }
        }

        public IEnumerable<T> GetItems()
        {
            if (Items == null)
            {
                for (int i = 0; i < 8; i++)
                {
                    foreach (var item in Nodes[i].GetItems())
                    {
                        yield return item;
                    }
                }

                yield break;
            }
            else
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    yield return Items[i];
                }
            }
        }
    }
}
