using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hatzap.Textures
{
    public static class TextureManager
    {
        static Dictionary<string, TextureMeta> availableTextures = new Dictionary<string, TextureMeta>();
        static Dictionary<string, Texture> loadedTextures = new Dictionary<string, Texture>();

        public static void Insert(IEnumerable<TextureMeta> metas)
        {
            foreach (var item in metas)
            {
                Insert(item);
            }
        }

        public static void Insert(TextureMeta meta)
        {
            if (availableTextures.ContainsKey(meta.FileName))
                return;

            availableTextures.Add(meta.FileName, meta);
        }

        public static bool Load(string name)
        {
            Texture t;

            if (!loadedTextures.TryGetValue(name, out t))
            {
                TextureMeta meta;

                if (availableTextures.TryGetValue(name, out meta))
                {
                    t = new Texture();
                    t.Load(meta);
                    loadedTextures.Add(name, t);
                }
            }

            return t != null;
        }

        public static void Unload(string name)
        {
            Texture t;

            if (loadedTextures.TryGetValue(name, out t))
            {
                t.Release();
                loadedTextures.Remove(name);
            }
        }

        public static Texture Get(string name)
        {
            Texture t;

            if (!loadedTextures.TryGetValue(name, out t))
            {
                TextureMeta meta;

                if (availableTextures.TryGetValue(name, out meta))
                {
                    t = new Texture();
                    t.Load(meta);
                    loadedTextures.Add(name, t);
                }
            }

            return t;
        }

        public static bool IsLoaded(string key)
        {
            return loadedTextures.ContainsKey(key);
        }

        public static bool IsAvailable(string key)
        {
            return availableTextures.ContainsKey(key);
        }
    }
}
