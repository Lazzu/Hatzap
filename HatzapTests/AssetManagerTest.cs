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
        const string testContents = "This is a test file.AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";

        AssetPackageHeader header = new AssetPackageHeader()
        {
            PackagePath = "test.zap",
            Assets = new List<AssetMeta>() {
                new AssetMeta()
                {
                    Path = "test.txt"
                },
            }
        };
        
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
            if (Directory.Exists("Assets"))
                Directory.Delete("Assets", true);
        }

        [TestMethod]
        public void CreateAndLoadPackage()
        {
            AssetManager.WritePackage(header);
            AssetManager.AddPackage("test.zap");
            var stream = AssetManager.GetStream("test.txt");

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
            AssetManager.InsertAssetProcessor(new AssetDataCompressionProcessor());

            AssetManager.WritePackage(header);
            AssetManager.AddPackage("test.zap");
            var stream = AssetManager.GetStream("test.txt");

            string text;
            using (StreamReader reader = new StreamReader(stream))
            {
                text = reader.ReadToEnd();
            }

            Assert.AreEqual(testContents, text, "The string read from the file should be equal to the written string.");
        }
    }
}
