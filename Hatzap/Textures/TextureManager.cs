using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using Hatzap.Assets;
using Hatzap.Utilities;
using OpenTK.Graphics.OpenGL;

namespace Hatzap.Textures
{
    public class TextureManager : AssetManagerBase<Texture>
    {
        protected override Texture LoadAsset(Stream stream)
        {
            byte[] metaBytes = null;
            byte[] rawBytes = null;
            int rawSize;
            using(BinaryReader br = new BinaryReader(stream, Encoding.UTF8))
            {
                var length = br.ReadInt32();
                metaBytes = br.ReadBytes(length);

                length = br.ReadInt32();          
                rawBytes = br.ReadBytes(length);

                rawSize = br.ReadInt32();
            }

            TextureMeta meta = null;
            Texture texture = null;

            // Uncompress header data
            using (MemoryStream ms = new MemoryStream(metaBytes))
            {
                using (GZipStream gz = new GZipStream(ms, CompressionMode.Decompress))
                {
                    meta = XML.Read.FromStream<TextureMeta>(gz);
                }
            }

            var pixels = new byte[rawSize];

            // Uncompress image data
            using (MemoryStream ms = new MemoryStream(rawBytes))
            {
                using (GZipStream gz = new GZipStream(ms, CompressionMode.Decompress))
                {
                    using(BinaryReader br = new BinaryReader(gz))
                    {
                        pixels = br.ReadBytes(rawSize);
                    }
                }
            }

            metaBytes = null;

            switch(meta.Type)
            {
                case TextureType.Texture2D:
                    texture = new Texture(meta);
                    break;
                case TextureType.TextureArray:
                    throw new NotImplementedException();
                    //texture = new TextureArray(meta);
                    break;
                case TextureType.TextureCubemap:
                    throw new NotImplementedException();
                    //texture = new CubeTexture(meta);
                    break;
                case TextureType.Texture3D:
                    throw new NotImplementedException();
                    break;
                case TextureType.Texture1D:
                    throw new NotImplementedException();
                    break;
                case TextureType.TextureArray3D:
                    throw new NotImplementedException();
                    break;
                case TextureType.TextureCubemapArray:
                    throw new NotImplementedException();
                    break;
                case TextureType.TextureRectangle:
                    throw new NotImplementedException();
                    break;
                default:
                    throw new Exception("Unknown texture format.");
            }

            texture.Load(pixels, meta);

            return texture;
        }

        protected override void SaveAsset(Texture asset, Stream stream)
        {
            byte[] metaBytes;

            // Compress header data
            using (MemoryStream ms = new MemoryStream())
            {
                using (GZipStream gz = new GZipStream(ms, CompressionLevel.Optimal))
                {
                    XML.Write.ToStream(asset.Metadata, gz);
                }

                metaBytes = ms.ToArray();
            }

            // Get raw texture bytes
            var rawPixels = asset.GetPixels(asset.Metadata);
            int rawSize = rawPixels.Length;

            byte[] rawBytes;

            // Compress texture data
            using (MemoryStream ms = new MemoryStream())
            {
                using (GZipStream gz = new GZipStream(ms, CompressionLevel.Optimal))
                {
                    gz.Write(rawPixels, 0, rawPixels.Length);
                }

                rawBytes = ms.ToArray();
            }

            using(BinaryWriter bw = new BinaryWriter(stream, Encoding.UTF8))
            {
                bw.Write(metaBytes.Length);
                bw.Write(metaBytes);
                bw.Write(rawBytes.Length);
                bw.Write(rawBytes);
                bw.Write(rawSize);
            }
        }
    }
}
