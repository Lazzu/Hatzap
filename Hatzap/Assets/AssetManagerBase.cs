using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hatzap.Assets
{
    /// <summary>
    /// The base class for asset managers.
    /// </summary>
    /// <typeparam name="T">The asset type the derived class is managing. Must be a reference type.</typeparam>
    public abstract class AssetManagerBase<T> where T : new()
    {
        protected Dictionary<string, T> loadedAssets = new Dictionary<string, T>();

        /// <summary>
        /// Insert an already loaded asset to the manager.
        /// </summary>
        /// <param name="path">Path of the asset</param>
        /// <param name="asset">The asset</param>
        public virtual void Insert(string path, T asset)
        {
            if (!loadedAssets.ContainsKey(path))
            {
                loadedAssets.Add(path, asset);
            }
        }

        public virtual void Remove(string path)
        {
            if(loadedAssets.ContainsKey(path))
            {
                loadedAssets.Remove(path);
            }
        }

        /// <summary>
        /// Saves the asset on disk.
        /// </summary>
        /// <param name="path">The asset path</param>
        /// <param name="targetPath">The asset target path on disk</param>
        public virtual void Save(string path, string targetPath)
        {
            T asset = Get(path, true);
            
            using(FileStream fs = new FileStream(targetPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                SaveAsset(asset, fs);
            }
        }
        
        /// <summary>
        /// Loads an asset to memory.
        /// </summary>
        /// <param name="path">Path to the asset.</param>
        /// <exception cref="FileNotFoundException">Thrown when the requested asset was unable to load.</exception>
        public virtual void Load(string path)
        {
            if (loadedAssets.ContainsKey(path))
            {
                return;
            }

            using (var stream = PackageManager.GetStream(path))
            {
                var asset = LoadAsset(stream);

                if(asset == null)
                {
                    throw new FileNotFoundException("Asset not found: " + path);
                }

                loadedAssets.Add(path, asset);
            }
        }

        /// <summary>
        /// Gets the asset requested. Loads the asset if autoload is set to true.
        /// </summary>
        /// <param name="path">The asset path</param>
        /// <param name="autoload">Automatically loads the asset to memory if set to true</param>
        /// <returns>The requested asset if it existed. Null if it did not exist.</returns>
        public virtual T Get(string path, bool autoload = false)
        {
            if(autoload)
            {
                try
                {
                    Load(path);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            }

            T asset = new T();
            loadedAssets.TryGetValue(path, out asset);

            return asset;
        }

        /// <summary>
        /// The asset-type spesific method that loads the assets in to memory.
        /// </summary>
        /// <param name="stream">The stream to the asset data</param>
        /// <returns>The successfully loaded asset, or null if load failed.</returns>
        protected abstract T LoadAsset(Stream stream);

        /// <summary>
        /// The asset-type spesific method for saving the asset on disk.
        /// </summary>
        /// <param name="asset">The asset to save.</param>
        /// <param name="stream">The stream to write the asset data in.</param>
        protected abstract void SaveAsset(T asset, Stream stream);
    }
}
