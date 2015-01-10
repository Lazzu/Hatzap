using System;
using OpenTK;

namespace Hatzap_Editor.ContentProcessors.Mesh
{
    /// <summary>
    /// Converts mesh data between Hatzap and Assimp meshes.
    /// 
    /// TODO: Animated meshes.
    /// </summary>
    public class AssimpConvertor
    {
        public Assimp.Mesh ToAssimp(Hatzap.Models.Mesh mesh)
        {
            var verts = mesh.Vertices;
            var norms = mesh.Normals;
            var tangents = mesh.Tangents;
            var binormals = mesh.Binormals;
            var uv = mesh.UV;
            var colors = mesh.Colors;

            var amesh = new Assimp.Mesh(Assimp.PrimitiveType.Triangle);

            for (int i = 0; i < verts.Length; i++)
            {
                if (verts != null) amesh.Vertices.Add(new Assimp.Vector3D(verts[i].X, verts[i].Y, verts[i].Z));
                if (norms != null) amesh.Normals.Add(new Assimp.Vector3D(norms[i].X, norms[i].Y, norms[i].Z));
                if (tangents != null) amesh.Tangents.Add(new Assimp.Vector3D(tangents[i].X, tangents[i].Y, tangents[i].Z));
                if (binormals != null) amesh.BiTangents.Add(new Assimp.Vector3D(binormals[i].X, binormals[i].Y, binormals[i].Z));
                if (uv != null)
                {
                    for (int j = 0; j < uv.Length; j++)
                    {
                        amesh.TextureCoordinateChannels[j].Add(new Assimp.Vector3D(uv[j][i].X, uv[j][i].Y, uv[j][i].Z));
                    }
                        
                }
                if (colors != null)
                {
                    for (int j = 0; j < uv.Length; j++)
                    {
                        amesh.VertexColorChannels[j].Add(new Assimp.Color4D(colors[j][i].X, colors[j][i].Y, colors[j][i].Z, colors[j][i].W));
                    }
                }
            }

            return amesh;
        }

        public Hatzap.Models.Mesh FromAssimp(Assimp.Mesh amesh)
        {
            var mesh = new Hatzap.Models.Mesh();

            var v = amesh.Vertices;
            var n = amesh.Normals;
            var t = amesh.Tangents;
            var b = amesh.BiTangents;
            var tc = amesh.TextureCoordinateChannels;
            var c = amesh.VertexColorChannels;

            var verts = new Vector3[v.Count];
            Vector3[] norms;
            Vector3[] tangents;
            Vector3[] binormals;
            Vector3[][] uv;
            Vector4[][] colors;

            if (amesh.HasNormals) norms = new Vector3[v.Count]; else norms = null;
            if (amesh.HasTangentBasis) tangents = new Vector3[v.Count]; else tangents = null;
            if (amesh.HasTangentBasis) binormals = new Vector3[v.Count]; else binormals = null;
            if (amesh.HasTextureCoords(0))
            {
                uv = new Vector3[amesh.TextureCoordinateChannelCount][];

                for(int i = 0; i < amesh.TextureCoordinateChannelCount; i++)
                {
                    uv[i] = new Vector3[v.Count];
                }

            }
            else
            {
                uv = null;
            }
            if (amesh.HasVertexColors(0))
            {
                colors = new Vector4[amesh.VertexColorChannelCount][];

                for (int i = 0; i < amesh.VertexColorChannelCount; i++)
                {
                    colors[i] = new Vector4[v.Count];
                }
            }
            else
            {
                colors = new Vector4[1][];
                colors[0] = new Vector4[v.Count];
            }

            for (int i = 0; i < v.Count; i++)
            {
                verts[i] = new Vector3(v[i].X, v[i].Y, v[i].Z);
                if (norms != null) norms[i] = new Vector3(n[i].X, n[i].Y, n[i].Z);
                if (tangents != null) tangents[i] = new Vector3(t[i].X, t[i].Y, t[i].Z);
                if (binormals != null) binormals[i] = new Vector3(b[i].X, b[i].Y, b[i].Z);
                if (uv != null)
                {
                    for(int j = 0; j < amesh.TextureCoordinateChannelCount; j++)
                    {
                        uv[j][i] = new Vector3(tc[j][i].X, tc[j][i].Y, tc[j][i].Z);
                    }
                }
                if (colors != null)
                {
                    if (amesh.HasVertexColors(0))
                    {
                        for (int j = 0; j < amesh.VertexColorChannelCount; j++)
                        {
                            colors[j][i] = new Vector4(c[j][i].R, c[j][i].G, c[j][i].B, c[j][i].A);
                        }
                    }
                    else
                    {
                        colors[0][i] = new Vector4(1, 1, 1, 1);
                    }
                    
                }
            }

            mesh.Vertices = verts;
            mesh.Normals = norms;
            mesh.Tangents = tangents;
            mesh.Binormals = binormals;
            mesh.UV = uv;
            mesh.Colors = colors;

            var aindices = amesh.GetIndices();

            var indices = new uint[aindices.Length];

            for (int i = 0; i < aindices.Length; i++)
            {
                indices[i] = (uint)aindices[i];
            }
            
            mesh.Indices = indices;

            return mesh;
        }
        
    }
}
