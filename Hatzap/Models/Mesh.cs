using System;
using System.Collections.Generic;
using System.Diagnostics;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Hatzap.Models
{
    public class Mesh
    {
        // GL Names
        int vao = 0, vertVbo = 0, normVbo = 0, tangentsVbo = 0, binormalsVbo = 0, uvVbo = 0, colorsVbo, ebo = 0;

        #region MeshData
        Vector3[] verts;
        Vector3[] norms;
        Vector3[] tangents;
        Vector3[] binormals;
        Vector3[] uv;
        Vector4[] colors;
        int[] indices;
        #endregion

        #region Dirtyness
        bool vertsDirty;
        bool normsDirty;
        bool tangentsDirty;
        bool binormalsDirty;
        bool uvDirty;
        bool colorsDirty;
        bool indicesDirty;
        #endregion

        #region Properties

        public Assimp.Mesh AssimpMesh
        {
            get
            {
                //throw new NotSupportedException("Converting mesh to Assimp.Mesh not supported.");

                var mesh = new Assimp.Mesh(Assimp.PrimitiveType.Triangle);

                for (int i = 0; i < verts.Length; i++)
                {
                    if (verts != null) mesh.Vertices.Add(new Assimp.Vector3D(verts[i].X, verts[i].Y, verts[i].Z));
                    if (norms != null) mesh.Normals.Add(new Assimp.Vector3D(norms[i].X, norms[i].Y, norms[i].Z));
                    if (tangents != null) mesh.Tangents.Add(new Assimp.Vector3D(tangents[i].X, tangents[i].Y, tangents[i].Z));
                    if (binormals != null) mesh.BiTangents.Add(new Assimp.Vector3D(binormals[i].X, binormals[i].Y, binormals[i].Z));
                    if (uv != null) mesh.TextureCoordinateChannels[0].Add(new Assimp.Vector3D(uv[i].X, uv[i].Y, 0));
                    if (colors != null) mesh.VertexColorChannels[0].Add(new Assimp.Color4D(colors[i].X, colors[i].Y, colors[i].Z, colors[i].W));
                }

                return mesh;
            }
            set
            {
                var mesh = value;

                var v = mesh.Vertices;
                var n = mesh.Normals;
                var t = mesh.Tangents;
                var b = mesh.BiTangents;
                var tc = mesh.TextureCoordinateChannels[0];
                var c = mesh.VertexColorChannels[0];

                verts = new Vector3[v.Count];

                if (mesh.HasNormals) norms = new Vector3[v.Count]; else norms = null;
                if (mesh.HasTangentBasis) tangents = new Vector3[v.Count]; else tangents = null;
                if (mesh.HasTangentBasis) binormals = new Vector3[v.Count]; else binormals = null;
                if (mesh.HasTextureCoords(0)) uv = new Vector3[v.Count]; else uv = null;
                if (mesh.HasVertexColors(0)) colors = new Vector4[v.Count]; else colors = null;

                for (int i = 0; i < v.Count; i++)
                {
                    verts[i] = new Vector3(v[i].X, v[i].Y, v[i].Z);
                    if (norms != null) norms[i] = new Vector3(n[i].X, n[i].Y, n[i].Z);
                    if (tangents != null) tangents[i] = new Vector3(t[i].X, t[i].Y, t[i].Z);
                    if (binormals != null) binormals[i] = new Vector3(b[i].X, b[i].Y, b[i].Z);
                    if (uv != null) uv[i] = new Vector3(tc[i].X, tc[i].Y, tc[i].Z);
                    if (colors != null) colors[i] = new Vector4(c[i].R, c[i].G, c[i].B, c[i].A);
                }

                if (verts != null) vertsDirty = true;
                if (norms != null) normsDirty = true;
                if (tangents != null) tangentsDirty = true;
                if (binormals != null) binormalsDirty = true;
                if (uv != null) uvDirty = true;
                if (colors != null) colorsDirty = true;

                indices = mesh.GetIndices();
                indicesDirty = true;
            }
        }

        /// <summary>
        /// If any part of the mesh needs to be uploaded, IsDirty is true.
        /// </summary>
        public bool IsDirty
        {
            get
            {
                return vertsDirty || normsDirty || tangentsDirty || binormalsDirty || uvDirty || colorsDirty || indicesDirty;
            }
        }
        
        /// <summary>
        /// If automatic uploading is true, every time the mesh is drawn, it will check if any parts of the mesh are dirty and uploads them if needed.
        /// </summary>
        public bool AutomaticUploading { get; set; }

        /// <summary>
        /// Array of mesh vertices.
        /// </summary>
        public Vector3[] Vertices
        {
            get
            {
                return verts;
            }
            set
            {
                vertsDirty = true;
                verts = value;
            }
        }
        
        /// <summary>
        /// Array of mesh normals.
        /// </summary>
        public Vector3[] Normals
        {
            get
            {
                return norms;
            }
            set
            {
                normsDirty = true;
                norms = value;
            }
        }
        
        /// <summary>
        /// Array of mesh tangents.
        /// </summary>
        public Vector3[] Tangents
        {
            get
            {
                return tangents;
            }
            set
            {
                tangentsDirty = true;
                tangents = value;
            }
        }
        
        /// <summary>
        /// Array of mesh binormals.
        /// </summary>
        public Vector3[] Binormals
        {
            get
            {
                return binormals;
            }
            set
            {
                binormalsDirty = true;
                binormals = value;
            }
        }
        
        /// <summary>
        /// Array of mesh texture coordinates.
        /// </summary>
        public Vector3[] UV
        {
            get
            {
                return uv;
            }
            set
            {
                uvDirty = true;
                uv = value;
            }
        }

        /// <summary>
        /// Array of mesh vertex colors
        /// </summary>
        public Vector4[] Colors
        {
            get
            {
                return colors;
            }
            set
            {
                colorsDirty = true;
                colors = value;
            }
        }

        /// <summary>
        /// Array of mesh indices.
        /// </summary>
        public int[] Indices
        {
            get
            {
                return indices;
            }
            set
            {
                indicesDirty = true;
                indices = value;
            }
        }

        /// <summary>
        /// Determines the buffer usage hint for each data buffer of the mesh. Defaults to static draw.
        /// </summary>
        public BufferUsageHint BufferUsageHint { get; set; }
        
        /// <summary>
        /// Determines what kind of primitive type is used when drawing the mesh. Defaults to triangles.
        /// </summary>
        public PrimitiveType Type { get; set; }

        #endregion

        
        public Mesh()
        {
            BufferUsageHint = OpenTK.Graphics.OpenGL.BufferUsageHint.StaticDraw;
            Type = PrimitiveType.Triangles;
            AutomaticUploading = true;
        }

        /// <summary>
        /// Uploads dirty parts of the mesh to the GPU.
        /// </summary>
        public void Upload()
        {
            if(vertsDirty && verts != null)
            {
                vertsDirty = false;

                if (vertVbo == 0) GL.GenBuffers(1, out vertVbo);

                UploadData(vertVbo, BufferTarget.ArrayBuffer, verts.Length * Vector3.SizeInBytes, verts);
            }

            if (normsDirty && norms != null)
            {
                normsDirty = false;

                if (normVbo == 0) GL.GenBuffers(1, out normVbo);

                UploadData(normVbo, BufferTarget.ArrayBuffer, norms.Length * Vector3.SizeInBytes, norms);
            }

            if (tangentsDirty && tangents != null)
            {
                tangentsDirty = false;

                if (tangentsVbo == 0) GL.GenBuffers(1, out tangentsVbo);

                UploadData(tangentsVbo, BufferTarget.ArrayBuffer, tangents.Length * Vector3.SizeInBytes, tangents);
            }

            if (binormalsDirty && binormals != null)
            {
                binormalsDirty = false;

                if (binormalsVbo == 0) GL.GenBuffers(1, out binormalsVbo);

                UploadData(binormalsVbo, BufferTarget.ArrayBuffer, binormals.Length * Vector3.SizeInBytes, binormals);
            }

            if (uvDirty && uv != null)
            {
                uvDirty = false;

                if (uvVbo == 0) GL.GenBuffers(1, out uvVbo);

                UploadData(uvVbo, BufferTarget.ArrayBuffer, uv.Length * Vector3.SizeInBytes, uv);
            }

            if (colorsDirty && colors != null)
            {
                colorsDirty = false;

                if (colorsVbo == 0) GL.GenBuffers(1, out colorsVbo);

                UploadData(colorsVbo, BufferTarget.ArrayBuffer, colors.Length * Vector4.SizeInBytes, colors);
            }

            if (indicesDirty && indices != null)
            {
                indicesDirty = false;

                if (ebo == 0) GL.GenBuffers(1, out ebo);

                UploadData(ebo, BufferTarget.ElementArrayBuffer, indices.Length * sizeof(int), indices);
            }
        }

        void UploadData<T>(int buffer, BufferTarget target, int size, T[] Data) where T : struct
        {
            GL.BindBuffer(target, buffer);
            GL.BufferData(target, new IntPtr(size), Data, this.BufferUsageHint);
            GL.BindBuffer(target, 0);
        }

        /// <summary>
        /// Draws the mesh. If automatic uploading is set to true and the mesh is dirty, Upload() will be called too.
        /// </summary>
        public void Draw()
        {
            if (verts == null)
            {
                Debug.WriteLine("Mesh being drawn has no vertices.");
                return;
            }   

            if (AutomaticUploading && IsDirty)
                Upload();

            if(indices != null)
            {
                bool mustDescribe = false;
                if(vao == 0)
                {
                    GL.GenVertexArrays(1, out vao);
                    mustDescribe = true;
                }
                
                GL.BindVertexArray(vao);
                
                if(mustDescribe)
                {
                    BindVertexAttribBuffer(0, vertVbo, BlittableValueType.StrideOf(verts), 3, VertexAttribPointerType.Float);
                    
                    if (norms != null)
                    {
                        BindVertexAttribBuffer(1, normVbo, BlittableValueType.StrideOf(norms), 3, VertexAttribPointerType.Float);
                    }

                    if (tangents != null)
                    {
                        BindVertexAttribBuffer(2, tangentsVbo, BlittableValueType.StrideOf(tangents), 3, VertexAttribPointerType.Float);
                    }

                    if (binormals != null)
                    {
                        BindVertexAttribBuffer(3, binormalsVbo, BlittableValueType.StrideOf(binormals), 3, VertexAttribPointerType.Float);
                    }

                    if (uv != null)
                    {
                        BindVertexAttribBuffer(4, uvVbo, BlittableValueType.StrideOf(uv), 3, VertexAttribPointerType.Float);
                    }

                    if (colors != null)
                    {
                        BindVertexAttribBuffer(5, colorsVbo, BlittableValueType.StrideOf(colors), 4, VertexAttribPointerType.Float);
                    }

                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
                }
                
                GL.DrawElements(Type, indices.Length, DrawElementsType.UnsignedInt, IntPtr.Zero);

                GL.BindVertexArray(0);
            }
            else
            {
                throw new NotSupportedException("Drawing non-indexed meshes not yet supported.");
            }
        }

        void BindVertexAttribBuffer(int attrib, int vbo, int stride, int elements, VertexAttribPointerType type)
        {
            GL.EnableVertexAttribArray(attrib);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.VertexAttribPointer(attrib, elements, type, false, stride, 0);
        }
    }
}
