using System;
using System.Collections.Generic;
using System.Diagnostics;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Hatzap.Models
{
    public class Mesh
    {
        Vector3[] verts;
        Vector3[] norms;
        Vector3[] tangents;
        Vector3[] binormals;
        Vector2[] uv;
        int[] indices;

        bool vertsDirty;
        bool normsDirty;
        bool tangentsDirty;
        bool binormalsDirty;
        bool uvDirty;
        bool indicesDirty;

        public bool IsDirty
        {
            get
            {
                return vertsDirty || normsDirty || tangentsDirty || binormalsDirty || uvDirty || indicesDirty;
            }
        }
        public bool AutomaticUploading { get; set; }

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

        public BufferUsageHint BufferUsageHint { get; set; }
        public PrimitiveType Type { get; set; }

        int vao = 0, vertVbo = 0, normVbo = 0, tangentsVbo = 0, binormalsVbo = 0, uvVbo = 0, ebo = 0;

        public Mesh()
        {
            BufferUsageHint = OpenTK.Graphics.OpenGL.BufferUsageHint.StaticDraw;
            Type = PrimitiveType.Triangles;
            AutomaticUploading = true;
        }

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

        public void Draw()
        {
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
                    Debug.WriteLine("Describing");

                    int attrib = 0;

                    GL.EnableVertexAttribArray(attrib);
                    GL.BindBuffer(BufferTarget.ArrayBuffer, vertVbo);
                    GL.VertexAttribPointer(attrib, 3, VertexAttribPointerType.Float, false, BlittableValueType.StrideOf(verts), 0);

                    attrib++;

                    if (norms != null)
                    {
                        GL.EnableVertexAttribArray(attrib);
                        GL.BindBuffer(BufferTarget.ArrayBuffer, normVbo);
                        GL.VertexAttribPointer(attrib, 3, VertexAttribPointerType.Float, false, BlittableValueType.StrideOf(norms), 0);
                        attrib++;
                    }

                    if (tangents != null)
                    {
                        GL.EnableVertexAttribArray(attrib);
                        GL.BindBuffer(BufferTarget.ArrayBuffer, tangentsVbo);
                        GL.VertexAttribPointer(attrib, 3, VertexAttribPointerType.Float, false, BlittableValueType.StrideOf(tangents), 0);
                        attrib++;
                    }

                    if (binormals != null)
                    {
                        GL.EnableVertexAttribArray(attrib);
                        GL.BindBuffer(BufferTarget.ArrayBuffer, binormalsVbo);
                        GL.VertexAttribPointer(attrib, 3, VertexAttribPointerType.Float, false, BlittableValueType.StrideOf(binormals), 0);
                        attrib++;
                    }

                    if (uv != null)
                    {
                        GL.EnableVertexAttribArray(attrib);
                        GL.BindBuffer(BufferTarget.ArrayBuffer, uvVbo);
                        GL.VertexAttribPointer(attrib, 2, VertexAttribPointerType.Float, false, BlittableValueType.StrideOf(uv), 0);
                        attrib++;
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
    }
}
