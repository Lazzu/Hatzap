using System;
using System.Collections.Generic;
using System.Diagnostics;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Hatzap.Models
{
    public class Mesh
    {
        #region MeshData
        Vector3[] verts;
        Vector3[] norms;
        Vector3[] tangents;
        Vector3[] binormals;
        Vector2[] uv;
        int[] indices;
        #endregion

        #region Dirtyness
        bool vertsDirty;
        bool normsDirty;
        bool tangentsDirty;
        bool binormalsDirty;
        bool uvDirty;
        bool indicesDirty;
        #endregion

        #region Properties
        /// <summary>
        /// If any part of the mesh needs to be uploaded, IsDirty is true.
        /// </summary>
        public bool IsDirty
        {
            get
            {
                return vertsDirty || normsDirty || tangentsDirty || binormalsDirty || uvDirty || indicesDirty;
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
        public Vector2[] UV
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

        // GL Names
        int vao = 0, vertVbo = 0, normVbo = 0, tangentsVbo = 0, binormalsVbo = 0, uvVbo = 0, ebo = 0;

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
                Debug.WriteLine("Uploading verts");
                vertsDirty = false;

                if (vertVbo == 0) GL.GenBuffers(1, out vertVbo);

                UploadData(vertVbo, BufferTarget.ArrayBuffer, verts.Length * Vector3.SizeInBytes, verts);
            }

            if (normsDirty && norms != null)
            {
                Debug.WriteLine("Uploading norms");
                normsDirty = false;

                if (normVbo == 0) GL.GenBuffers(1, out normVbo);

                UploadData(normVbo, BufferTarget.ArrayBuffer, norms.Length * Vector3.SizeInBytes, norms);
            }

            if (tangentsDirty && tangents != null)
            {
                Debug.WriteLine("Uploading tangents");
                tangentsDirty = false;

                if (tangentsVbo == 0) GL.GenBuffers(1, out tangentsVbo);

                UploadData(tangentsVbo, BufferTarget.ArrayBuffer, tangents.Length * Vector3.SizeInBytes, tangents);
            }

            if (binormalsDirty && binormals != null)
            {
                Debug.WriteLine("Uploading binormals");
                binormalsDirty = false;

                if (binormalsVbo == 0) GL.GenBuffers(1, out binormalsVbo);

                UploadData(binormalsVbo, BufferTarget.ArrayBuffer, binormals.Length * Vector3.SizeInBytes, binormals);
            }

            if (uvDirty && uv != null)
            {
                Debug.WriteLine("Uploading uv");
                uvDirty = false;

                if (uvVbo == 0) GL.GenBuffers(1, out uvVbo);

                UploadData(uvVbo, BufferTarget.ArrayBuffer, uv.Length * Vector2.SizeInBytes, uv);
            }

            if (indicesDirty && indices != null)
            {
                Debug.WriteLine("Uploading indices");
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
                    int attrib = 0;

                    BindVertexAttribBuffer(attrib, vertVbo, BlittableValueType.StrideOf(verts), 3, VertexAttribPointerType.Float);

                    attrib++;

                    if (norms != null)
                    {
                        BindVertexAttribBuffer(attrib, normVbo, BlittableValueType.StrideOf(norms), 3, VertexAttribPointerType.Float);
                        attrib++;
                    }

                    if (tangents != null)
                    {
                        BindVertexAttribBuffer(attrib, tangentsVbo, BlittableValueType.StrideOf(tangents), 3, VertexAttribPointerType.Float);
                        attrib++;
                    }

                    if (binormals != null)
                    {
                        BindVertexAttribBuffer(attrib, binormalsVbo, BlittableValueType.StrideOf(binormals), 3, VertexAttribPointerType.Float);
                        attrib++;
                    }

                    if (uv != null)
                    {
                        BindVertexAttribBuffer(attrib, uvVbo, BlittableValueType.StrideOf(uv), 2, VertexAttribPointerType.Float);
                        attrib++;
                    }

                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
                }
                
                GL.DrawElements(Type, indices.Length, DrawElementsType.UnsignedInt, IntPtr.Zero);

                //GL.BindVertexArray(0);
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
