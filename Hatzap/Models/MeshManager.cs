using System;
using System.Collections.Generic;
using System.IO;
using Hatzap.Assets;
using OpenTK;

namespace Hatzap.Models
{
    public class MeshManager : AssetManagerBase<Mesh>
    {
        protected override Mesh LoadAsset(Stream stream)
        {
            var br = new BinaryReader(stream);

            var vcount = br.ReadInt32();
            var hasnormals = br.ReadBoolean();
            var hastangents = br.ReadBoolean();
            var hasbinormals = br.ReadBoolean();
            var hascolors = br.ReadBoolean();
            var ccount = br.ReadInt32();
            var hasuv = br.ReadBoolean();
            var ucount = br.ReadInt32();
            var icount = br.ReadInt32();

            Vector3[] vertices = new Vector3[vcount];
            Vector3[] normals = null;
            Vector3[] tangents = null;
            Vector3[] binormals = null;
            Vector4[][] colors = null;
            Vector3[][] uv = null;
            uint[] indices = null;

            if(icount > 0)
            {
                indices = new uint[icount];
            }

            if (hasnormals)
                normals = new Vector3[vcount];

            if (hastangents)
                tangents = new Vector3[vcount];

            if (hasbinormals)
                binormals = new Vector3[vcount];

            if (hascolors)
            {
                colors = new Vector4[ccount][];
                for (int i = 0; i < ccount; i++)
                {
                    colors[i] = new Vector4[vcount];
                }
            }

            if (hasuv)
            {
                uv = new Vector3[ucount][];
                for (int i = 0; i < ucount; i++)
                {
                    uv[i] = new Vector3[vcount];
                }
            }

            for (int i = 0; i < vcount; i++)
            {
                vertices[i] = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());

                if (hasnormals)
                {
                    normals[i] = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                }

                if (hastangents)
                {
                    tangents[i] = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                }

                if (hasbinormals)
                {
                    binormals[i] = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                }

                if (hascolors)
                {
                    for (int j = 0; j < ccount; j++)
                    {
                        colors[j][i] = new Vector4(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                    }
                }

                if (hasuv)
                {
                    for (int j = 0; j < ucount; j++)
                    {
                        uv[j][i] = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                    }
                }
            }

            for (int i = 0; i < icount; i++)
            {
                indices[i] = br.ReadUInt32();
            }

            Mesh mesh = new Mesh()
            {
                Vertices = vertices,
                Normals = normals,
                Tangents = tangents,
                Binormals = binormals,
                Colors = colors,
                UV = uv,
                Indices = indices,
            };

            return mesh;
        }

        /// <summary>
        /// This is a temporary function until something better becomes available. Will not be included in the final API.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="stream"></param>
        public void TemporarySaveAsset(Mesh mesh, Stream stream)
        {
            SaveAsset(mesh, stream);
        }

        protected override void SaveAsset(Mesh mesh, Stream stream)
        {
            BinaryWriter bw = new BinaryWriter(stream);

            var vertices = mesh.Vertices;
            var normals = mesh.Normals;
            var tangents = mesh.Tangents;
            var binormals = mesh.Binormals;
            var colors = mesh.Colors;
            var uv = mesh.UV;
            var indices = mesh.Indices;

            var vcount = vertices.Length;
            var ccount = colors == null ? 0 : colors.Length;
            var ucount = uv == null ? 0 : uv.Length;
            var icount = indices == null ? 0 : indices.Length;

            bw.Write(vcount);
            bw.Write(normals != null);
            bw.Write(tangents != null);
            bw.Write(binormals != null);
            bw.Write(colors != null);
            bw.Write(ccount);
            bw.Write(uv != null);
            bw.Write(ucount);
            bw.Write(icount);

            for(int i = 0; i < vcount; i++)
            {
                bw.Write(vertices[i].X);
                bw.Write(vertices[i].Y);
                bw.Write(vertices[i].Z);

                if(normals != null)
                {
                    bw.Write(normals[i].X);
                    bw.Write(normals[i].Y);
                    bw.Write(normals[i].Z);
                }

                if (tangents != null)
                {
                    bw.Write(tangents[i].X);
                    bw.Write(tangents[i].Y);
                    bw.Write(tangents[i].Z);
                }

                if (binormals != null)
                {
                    bw.Write(binormals[i].X);
                    bw.Write(binormals[i].Y);
                    bw.Write(binormals[i].Z);
                }

                if (colors != null)
                {
                    for (int j = 0; j < ccount; j++)
                    {
                        bw.Write(colors[j][i].X);
                        bw.Write(colors[j][i].Y);
                        bw.Write(colors[j][i].Z);
                        bw.Write(colors[j][i].W);
                    }
                }

                if (uv != null)
                {
                    for (int j = 0; j < ucount; j++)
                    {
                        bw.Write(uv[j][i].X);
                        bw.Write(uv[j][i].Y);
                        bw.Write(uv[j][i].Z);
                    }
                }
            }

            for(int i = 0; i < icount; i++)
            {
                bw.Write(indices[i]);
            }

        }

        public override void ReleaseAll()
        {
            foreach (var item in loadedAssets)
            {
                item.Value.Release();
            }
        }
    }
}
