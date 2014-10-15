using System;
using System.Collections.Generic;
using Hatzap.Models;
using Hatzap.Rendering;
using Hatzap.Utilities;
using OpenTK;

namespace Hatzap.Scenes
{
    public static class SceneManager
    {
        static OctTree<ITransformable> tree;

        public static int ObjectCount { get; set; }

        public static bool CullByObject { get; set; }

        public static void Initialize(float initialChunkSize, int initialMaxDepth, int nodeCapacity, Vector3 center)
        {
            initialChunkSize = initialChunkSize / 2;

            tree = new OctTree<ITransformable>(nodeCapacity, initialMaxDepth, new List<ITransformable>(), new BoundingBox(new Vector3(-initialChunkSize) + center, new Vector3(initialChunkSize) + center));
        }

        public static void Insert(ITransformable item)
        {
            if (tree == null) throw new Exception("SceneManager needs to be initialized before using any of it's methods.");
            if (item == null) throw new ArgumentException("Inserted item can not be null!", "item");
            
            if (tree.Insert(item))
                ObjectCount++;
        }

        public static void Insert(IEnumerable<ITransformable> items)
        {
            if (tree == null) throw new Exception("SceneManager needs to be initialized before using any of it's methods.");
            if (items == null) return;

            foreach (var item in items)
            {
                if (item != null)
                {
                    if (tree.Insert(item))
                        ObjectCount++;
                }
            }
        }

        public static void Remove(ITransformable item)
        {
            tree.Remove(item);
        }

        public static void Update()
        {
            Time.StartTimer("SceneManager.Update()", "Update");

            // Get non-static root objects
            foreach (var item in tree.GetItems(i => i.Transform.Parent == null && (!i.Transform.Static || !i.Transform.calculated)))
            {
                CalculateMatrices(item);
            }

            Time.StopTimer("SceneManager.Update()");
        }

        static void CalculateMatrices(ITransformable item)
        {
            item.Transform.CalculateMatrix();

            foreach (var child in item.GetChildren(i => (!i.Transform.Static || !i.Transform.calculated)))
            {
                CalculateMatrices(child);
            }
        }

        public static void QueueForRendering(Camera camera)
        {
            Time.StartTimer("SceneManager.QueueForRendering(Camera)", "Update");

            foreach (var item in tree.GetItems(camera.Frustum, CullByObject))
            {
                var tmp = item as Renderable;

                if(tmp != null)
                {
                    RenderQueue.Insert(tmp);
                }
            }

            Time.StopTimer("SceneManager.QueueForRendering(Camera)");
        }
    }
}
