using System;
using System.Collections.Generic;
using System.Diagnostics;
using Hatzap.Utilities;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Hatzap.Models
{
    public class Mesh
    {
        // GL Names
        int vao = 0, vertVbo = 0, normVbo = 0, tangentsVbo = 0, binormalsVbo = 0, uvVbo = 0, colorsVbo, ebo = 0;

        public int Triangles { get; protected set; }

        #region MeshData
        Vector3[] verts;
        Vector3[] norms;
        Vector3[] tangents;
        Vector3[] binormals;
        Vector3[][] uv;
        Vector4[][] colors;
        uint[] indices;
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

        public int VertexAttribLocation { get; set; }
        public int NormalAttribLocation { get; set; }
        public int TangentAttribLocation { get; set; }
        public int BinormalAttribLocation { get; set; }
        public int UVAttribLocation { get; set; }
        public int ColorAttribLocation { get; set; }

        public BoundingBox CalculatedBoundingbox
        {
            get
            {
                Vector3 min = verts[0];
                Vector3 max = verts[0];

                for (int i = 1; i < verts.Length; i++)
                {
                    var vert = verts[i];

                    if (vert.X < min.X)
                        min.X = vert.X;
                    
                    if (vert.X > max.X)
                        max.X = vert.X;

                    if (vert.Y < min.Y)
                        min.Y = vert.Y;
                    
                    if (vert.Y > max.Y)
                        max.Y = vert.Y;

                    if (vert.Z < min.Z)
                        min.Z = vert.Z;
                    
                    if (vert.Z > max.Z)
                        max.Z = vert.Z;
                }
                
                return new BoundingBox(min, max);
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
        public Vector3[][] UV
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
        public Vector4[][] Colors
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
        public uint[] Indices
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
            VertexAttribLocation = -1;
            NormalAttribLocation = -1;
            TangentAttribLocation = -1;
            BinormalAttribLocation = -1;
            UVAttribLocation = -1;
            ColorAttribLocation = -1;
        }

        /// <summary>
        /// Uploads dirty parts of the mesh to the GPU.
        /// </summary>
        public void Upload()
        {
            if(vertsDirty && verts != null && VertexAttribLocation > -1)
            {
                vertsDirty = false;

                if (vertVbo == 0) GL.GenBuffers(1, out vertVbo);

                UploadData(vertVbo, BufferTarget.ArrayBuffer, verts.Length * Vector3.SizeInBytes, verts);
            }

            if (normsDirty && norms != null && NormalAttribLocation > -1)
            {
                normsDirty = false;

                if (normVbo == 0) GL.GenBuffers(1, out normVbo);

                UploadData(normVbo, BufferTarget.ArrayBuffer, norms.Length * Vector3.SizeInBytes, norms);
            }

            if (tangentsDirty && tangents != null && TangentAttribLocation > -1)
            {
                tangentsDirty = false;

                if (tangentsVbo == 0) GL.GenBuffers(1, out tangentsVbo);

                UploadData(tangentsVbo, BufferTarget.ArrayBuffer, tangents.Length * Vector3.SizeInBytes, tangents);
            }

            if (binormalsDirty && binormals != null && BinormalAttribLocation > -1)
            {
                binormalsDirty = false;

                if (binormalsVbo == 0) GL.GenBuffers(1, out binormalsVbo);

                UploadData(binormalsVbo, BufferTarget.ArrayBuffer, binormals.Length * Vector3.SizeInBytes, binormals);
            }

            if (uvDirty && uv != null && UVAttribLocation > -1)
            {
                uvDirty = false;

                if (uvVbo == 0) GL.GenBuffers(1, out uvVbo);

                // TODO: Support multiple uv channels
                UploadData(uvVbo, BufferTarget.ArrayBuffer, uv[0].Length * Vector3.SizeInBytes, uv[0]);
            }

            if (colorsDirty && colors != null && ColorAttribLocation > -1)
            {
                colorsDirty = false;

                if (colorsVbo == 0) GL.GenBuffers(1, out colorsVbo);

                // TODO: Support multiple color channels
                UploadData(colorsVbo, BufferTarget.ArrayBuffer, colors[0].Length * Vector4.SizeInBytes, colors[0]);
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
            //GL.BufferSubData(target, IntPtr.Zero, new IntPtr(size), Data);
            GL.BufferData(target, new IntPtr(size), Data, this.BufferUsageHint);
            GL.BindBuffer(target, 0);
        }

        /// <summary>
        /// Draws the mesh. If automatic uploading is set to true and the mesh is dirty, Upload() will be called too.
        /// </summary>
        public void Draw()
        {
            Time.StartTimer("Mesh.Draw()", "Render");
            if (verts == null)
            {
                Debug.WriteLine("Mesh being drawn has no vertices.");
                return;
            }   

            if(indices != null)
            {
                Bind();
                
                GL.DrawElements(Type, indices.Length, DrawElementsType.UnsignedInt, IntPtr.Zero);

                GL.BindVertexArray(0);
            }
            else
            {
                throw new NotSupportedException("Drawing non-indexed meshes not yet supported.");
            }
            Time.StopTimer("Mesh.Draw()");
        }

        public void DrawInstanced(int count)
        {
            Time.StartTimer("Mesh.DrawInstanced()", "Render");
            GL.DrawElementsInstancedBaseVertex(Type, indices.Length, DrawElementsType.UnsignedInt, IntPtr.Zero, count, 0);
            Time.StopTimer("Mesh.DrawInstanced()");
        }

        /// <summary>
        /// When drawing instanced, you need to call this before the draw call
        /// </summary>
        public void Bind()
        {
            if (AutomaticUploading && IsDirty)
                Upload();

            bool mustDescribe = false;

            if (vao == 0)
            {
                GL.GenVertexArrays(1, out vao);
                mustDescribe = true;
            }

            GL.BindVertexArray(vao);

            if (mustDescribe)
            {
                if (VertexAttribLocation < 0)
                    throw new Exception("Invalid VertexAttribLocation");

                BindVertexAttribBuffer(VertexAttribLocation, vertVbo, BlittableValueType.StrideOf(verts), 3, VertexAttribPointerType.Float);

                if (norms != null && NormalAttribLocation > -1)
                {
                    BindVertexAttribBuffer(NormalAttribLocation, normVbo, BlittableValueType.StrideOf(norms), 3, VertexAttribPointerType.Float);
                }

                if (tangents != null && TangentAttribLocation > -1)
                {
                    BindVertexAttribBuffer(TangentAttribLocation, tangentsVbo, BlittableValueType.StrideOf(tangents), 3, VertexAttribPointerType.Float);
                }

                if (binormals != null && BinormalAttribLocation > -1)
                {
                    BindVertexAttribBuffer(BinormalAttribLocation, binormalsVbo, BlittableValueType.StrideOf(binormals), 3, VertexAttribPointerType.Float);
                }

                if (uv != null && UVAttribLocation > -1)
                {
                    BindVertexAttribBuffer(UVAttribLocation, uvVbo, BlittableValueType.StrideOf(uv[0]), 3, VertexAttribPointerType.Float);
                }

                if (colors != null && ColorAttribLocation > -1)
                {
                    BindVertexAttribBuffer(ColorAttribLocation, colorsVbo, BlittableValueType.StrideOf(colors[0]), 4, VertexAttribPointerType.Float);
                }

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            }

        }

        public void UnBind()
        {
            GL.BindVertexArray(0);
        }

        void BindVertexAttribBuffer(int attrib, int vbo, int stride, int elements, VertexAttribPointerType type)
        {
            GL.EnableVertexAttribArray(attrib);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.VertexAttribPointer(attrib, elements, type, false, stride, 0);
        }
    }
}
