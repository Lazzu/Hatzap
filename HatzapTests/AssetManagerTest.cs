using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Hatzap.Assets;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HatzapTests
{
    [TestClass]
    public class AssetManagerTest
    {
        const string testContents = "This is a test file.\r\nAAAAAAAAAAAAAAAAAAAAAAAAABAAAAAAAAAAAAAAAAAAAAAAabcdefghijklmnopqrstuvwxyzåäöabcdefghijklmnopqrstuvwxyzåäöabcdefghijklmnopqrstuvwxyzåäöabcdefghijklmnopqrstuvwxyzåäöabcdefghijklmnopqrstuvwxyzåäöabcdefghijklmnopqrstuvwxyzåäöabcdefghijklmnopqrstuvwxyzåäöabcdefghijklmnopqrstuvwxyzåäöabcdefghijklmnopqrstuvwxyzåäöabcdefghijklmnopqrstuvwxyzåäöabcdefghijklmnopqrstuvwxyzåäö";

        [TestInitialize]
        public void Initialize()
        {
            if (!Directory.Exists("Assets"))
                Directory.CreateDirectory("Assets");

            File.WriteAllText("Assets/test.txt", testContents, Encoding.UTF8);
        }

        [TestCleanup]
        public void Cleanup()
        {
            AssetManager.Clear();

            if (Directory.Exists("Assets"))
                Directory.Delete("Assets", true);
        }

        [TestMethod]
        public void LoadNonpackagedFiles()
        {
            // Get a stream to an asset
            var stream = AssetManager.GetStream("test.txt");

            Assert.IsNotNull(stream, "The stream returned should not be null.");

            // Read the asset from the stream
            string text;
            using (StreamReader reader = new StreamReader(stream))
            {
                text = reader.ReadToEnd();
            }

            Assert.AreEqual(testContents, text, "The string read from the file should be equal to the written string.");
        }

        [TestMethod]
        public void CreateAndLoadPackage()
        {
            // Specify the header information
            AssetPackageHeader header = new AssetPackageHeader()
            {
                PackagePath = "test.zap", // The package path and file name
                Assets = new List<AssetMeta>() {
                    new AssetMeta()
                    {
                        Path = "test.txt" // Path to a file on disk
                    },
                }
            };

            // Write asset package to disk
            AssetManager.WritePackage(header);

            // Read asset package headers to the asset manager asset registry
            AssetManager.AddPackage("test.zap");

            // Get a stream to an asset
            var stream = AssetManager.GetStream("test.txt");

            Assert.IsNotNull(stream, "The stream returned should not be null.");

            // Read the asset from the stream
            string text;
            using (StreamReader reader = new StreamReader(stream))
            {
                text = reader.ReadToEnd();
            }

            Assert.AreEqual(testContents, text, "The string read from the file should be equal to the written string.");
        }

        [TestMethod]
        public void CompressedPackage()
        {
            // Specify the header information
            AssetPackageHeader header = new AssetPackageHeader()
            {
                PackagePath = "test.zap", // The package path and file name
                Assets = new List<AssetMeta>() {
                    new AssetMeta()
                    {
                        Path = "test.txt" // Path to a file on disk
                    },
                }
            };

            // Insert asset data compression processor to the asset manager
            AssetManager.InsertAssetProcessor(new AssetDataCompressionProcessor(System.IO.Compression.CompressionLevel.Optimal));

            // Build asset package to disk from asset files on disk
            AssetManager.WritePackage(header);

            // Read asset package headers to the asset manager asset registry
            AssetManager.AddPackage("test.zap");

            // Get a stream to an asset
            var stream = AssetManager.GetStream("test.txt");

            Assert.IsNotNull(stream, "The stream returned should not be null.");

            // Read the asset from the stream
            string text;
            using (StreamReader reader = new StreamReader(stream))
            {
                text = reader.ReadToEnd();
            }

            Assert.AreEqual(testContents, text, "The string read from the file should be equal to the written string.");
        }
    }
}
