using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using Hatzap.Utilities;

namespace Hatzap.Assets
{
    public static class PackageManager
    {
        static Dictionary<string, Stream> packageStreams = new Dictionary<string, Stream>();
        static Dictionary<string, AssetMeta> assetPaths = new Dictionary<string, AssetMeta>();

        static List<IAssetDataProcessor> packageProcessors = new List<IAssetDataProcessor>();
        static List<IAssetDataProcessor> assetProcessors = new List<IAssetDataProcessor>();

        static string appPath = string.Empty;
        static string basePath = "Assets/";
        
        /// <summary>
        /// The relative base path for all assets
        /// </summary>
        public static string BasePath
        {
            get { return basePath; }
            set { basePath = value; }
        }

        /// <summary>
        /// Inserts a package processor to process the whole packages with.
        /// </summary>
        /// <param name="processor">The data processor.</param>
        public static void InsertPackageProcessor(IAssetDataProcessor processor)
        {
            packageProcessors.Add(processor);
        }

        /// <summary>
        /// Inserts an asset processor to process the assets in packages with.
        /// </summary>
        /// <param name="processor">The data processor.</param>
        public static void InsertAssetProcessor(IAssetDataProcessor processor)
        {
            assetProcessors.Add(processor);
        }

        /// <summary>
        /// Adds a package to the AssetManager package registry. Allows assets to be loaded from the packages.
        /// </summary>
        /// <param name="path">The path to the package inside BasePath.</param>
        public static void AddPackage(string path)
        {
            // Create stream from the package file
            using (var package = GetStream(path))
            {
                using (var processed = SetPackageReadProcessors(package))
                {
                    // Read header
                    package.Seek(0, SeekOrigin.Begin);
                    AssetPackageHeader header = new AssetPackageHeader();
                    header.Read(processed);
                    foreach (var item in header.Assets)
                    {
                        item.Package = path;
                    }
                    AddPackage(header);
                }
            }
        }

        public static void AddPackage(AssetPackageHeader header)
        {
            // Insert assets to the dictionary
            foreach (var asset in header.Assets)
            {
                assetPaths.Add(asset.Path, asset);
            }
        }

        /// <summary>
        /// Get a stream to an asset or package. 
        /// 
        /// If the file is in a package, the data is loaded to memory and a memorystream is returned. Also the 
        /// package is opened and should be closed after no more loading will be done.
        /// 
        /// If the file is not in a currently loaded package, a stream to that file in filesystem is returned.
        /// </summary>
        /// <param name="path">The file's unique path.</param>
        /// <returns>A stream to the file's data.</returns>
        public static Stream GetStream(string path)
        {
            if (path == null)
                throw new ArgumentNullException("Path can not be null!");

            if (path == string.Empty)
                throw new ArgumentException("Path can not be empty!");

            if (path.Contains(".."))
                throw new ArgumentException("Path must not contain double dots (\"..\").");

            if (assetPaths.ContainsKey(path))
            {
                return GetStreamFromPackage(path);
            }

            return GetStreamFromFileSystem(path);
        }

        /// <summary>
        /// Gets a stream from the file in the assets folder
        /// </summary>
        /// <param name="path">The path to the file inside the assets folder</param>
        /// <returns>A stream to the file</returns>
        private static Stream GetStreamFromFileSystem(string path)
        {
            if (appPath == string.Empty)
            {
                appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            }

            path = Path.Combine(appPath, basePath, path);

            if (!File.Exists(path))
                throw new FileNotFoundException("File not found: " + path);

            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None);

            return fs;
        }

        private static Stream GetStreamFromPackage(string path)
        {
            // Get asset meta
            var meta = assetPaths[path];

            // Get the package stream and seek to the correct position
            Stream package;
            if (!packageStreams.TryGetValue(meta.Package, out package))
            {
                OpenPackage(meta.Package);
                package = packageStreams[meta.Package];
            }
            package.Seek(meta.Position, SeekOrigin.Begin);

            // Read the data in a byte array
            byte[] data;
            using (BinaryReader binread = new BinaryReader(package, Encoding.UTF8, true))
            {
                data = binread.ReadBytes((int)meta.Size);
            }

            Debug.WriteLine("Read data from package: " + data.Length + " bytes");

            // Create a stream from the byte array and apply read processors
            Stream readProcessorStream = SetAssetReadProcessors(new MemoryStream(data));

            return readProcessorStream;
        }

        /// <summary>
        /// Opens a package for reading to speed up reading assets.
        /// </summary>
        /// <param name="path">The package path</param>
        public static void OpenPackage(string path)
        {
            Stream stream;
            if(!packageStreams.TryGetValue(path, out stream))
            {
                stream = GetStream(path);
                stream = SetPackageReadProcessors(stream);
                packageStreams.Add(path, stream);
            }
        }

        /// <summary>
        /// Closes a package to free up system resources.
        /// </summary>
        /// <param name="path">The package path</param>
        public static void ClosePackage(string path)
        {
            Stream stream;
            if (packageStreams.TryGetValue(path, out stream))
            {
                stream.Dispose();
                packageStreams.Remove(path);
            }
        }

        /// <summary>
        /// Closes all open packages to free up system resources.
        /// </summary>
        public static void CloseAllPackages()
        {
            foreach (var item in packageStreams)
            {
                item.Value.Dispose();
            }
            packageStreams.Clear();
        }

        /// <summary>
        /// Clear the asset registry and processors.
        /// </summary>
        public static void Clear()
        {
            CloseAllPackages();
            assetPaths.Clear();
            assetProcessors.Clear();
            packageProcessors.Clear();
        }

        /// <summary>
        /// Writes a package file using the header information
        /// </summary>
        /// <param name="header"></param>
        public static void WritePackage(AssetPackageHeader header)
        {
            var path = Path.Combine(appPath, basePath, header.PackagePath);

            Stream package = File.Create(path);
            
            using(package = SetPackageWriteProcessors(package))
            {
                // Write file paths and placeholder sizes and positions so that we can start writing the files themselves.
                header.Write(package);
                
                foreach (var asset in header.Assets)
                {
                    // Get the asset full path in file system
                    var assetPath = Path.Combine(appPath, basePath, asset.Path);

                    using(MemoryStream ms = new MemoryStream())
                    {
                        // Set asset position before write
                        asset.Position = package.Position;
                        
                        // Process them bytes, nao!
                        using(var assetStream = SetAssetWriteProcessors(ms))
                        {
                            var bytes = File.ReadAllBytes(assetPath);
                            assetStream.Write(bytes, 0, bytes.Length);
                        }

                        // Get processed bytes and write them to the file
                        var processedBytes = ms.ToArray();
                        package.Write(processedBytes, 0, processedBytes.Length);

                        // Set asset size
                        asset.Size = processedBytes.Length;
                    }
                }
                
                // Return to the start of the file
                package.Seek(0, SeekOrigin.Begin);

                // Rewrite file header with the correct sizes and positions
                header.Write(package);
            }
        }

        private static Stream SetPackageReadProcessors(Stream package)
        {
            for (int i = 0; i < packageProcessors.Count; i++)
            {
                package = packageProcessors[i].PackagaeRead(package);
            }
            return package;
        }

        private static Stream SetAssetReadProcessors(Stream asset)
        {
            for (int i = 0; i < assetProcessors.Count; i++)
            {
                asset = assetProcessors[i].AssetRead(asset);
            }
            return asset;
        }

        private static Stream SetPackageWriteProcessors(Stream package)
        {
            for (int i = packageProcessors.Count - 1; i >= 0; i--)
            {
                package = packageProcessors[i].PackagaeWrite(package);
            }
            return package;
        }

        private static Stream SetAssetWriteProcessors(Stream asset)
        {
            for (int i = assetProcessors.Count - 1; i >= 0; i--)
            {
                asset = assetProcessors[i].AssetWrite(asset);
            }
            return asset;
        }

        
    }
}
