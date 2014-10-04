using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hatzap.Rendering
{
    public static class RenderDataPool
    {
        static List<RenderData> pool = new List<RenderData>();
        static int count = 0;

        public static int Count { get { return count; } }
        public static int MaxItems { get; set; }

        public static void CreateReserve(int n)
        {
            if (MaxItems < n)
                MaxItems = n;

            for(int i = 0; i < n; i++)
            {
                Release(new RenderData());
            }
        }

        public static RenderData GetInstance()
        {
            if (pool.Count == 0)
                return new RenderData();

            int index = count - 1;

            var item = pool[index];
            pool[index] = null;

            count--;

            return item;
        }

        public static void Release(RenderData data)
        {
            data.RenderObject = null;

            if (count >= MaxItems)
                return;

            if(pool.Count > count)
            {
                pool[count] = data;
            }
            else
            {
                pool.Add(data);
            }
            count++;
        }
    }
}
