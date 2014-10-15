using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hatzap.Utilities;
using OpenTK;

namespace Hatzap.Models
{
    /// <summary>
    /// Holds game object's position, rotation and scale.
    /// </summary>
    public class Transform
    {
        Transform parent = null;

        /// <summary>
        /// The transform's parent the current transform is relative to.
        /// </summary>
        public Transform Parent { get { return parent; } set { parent = value; calculated = false; } }

        public Vector3 Position = Vector3.Zero;
        public Quaternion Rotation = Quaternion.Identity;
        public Vector3 Scale = Vector3.One;

        /// <summary>
        /// Contains the transform matrix. If parent exists, the transform is in relation to the parent.
        /// </summary>
        public Matrix4 Matrix = Matrix4.Identity;

        /// <summary>
        /// Contains the transform matrix in local space without the relation to parent.
        /// </summary>
        public Matrix4 LocalMatrix = Matrix4.Identity;

        Matrix4 translate;
        Matrix4 rotate;
        Matrix4 scale;
        Matrix4 scaleRotate;
        Vector3 axis;
        float angle;

        /// <summary>
        /// If IRenderable is static, the Transform's matrix is calculated only the first time the matrix is needed.
        /// The matrix will not be recalculated even when the parent transform changes.
        /// </summary>
        public bool Static { get; set; }
        internal bool calculated = false;

        /// <summary>
        /// Calculates the matrix for this transformation. If parent is set, it's matrix is applied to this transform's matrix.
        /// If Transform is static and the transform matrix has already been calculated previously, nothig happens. This method
        /// also assumes that parents' matrices have already been calculated.
        /// </summary>
        internal void CalculateMatrix()
        {
            Time.StartTimer("Transform.CalculateMatrix()", "Update");

            if (Static && calculated)
            {
                Time.StopTimer("Transform.CalculateMatrix()");
                return;
            }
            calculated = true;
            
            Matrix4.CreateTranslation(ref Position, out translate);
            Rotation.ToAxisAngle(out axis, out angle);
            Matrix4.CreateFromAxisAngle(axis, angle, out rotate);
            Matrix4.CreateScale(ref Scale, out scale);
            Matrix4.Mult(ref scale, ref rotate, out scaleRotate);
            Matrix4.Mult(ref scaleRotate, ref translate, out LocalMatrix);

            if (parent != null)
            {
                Matrix4.Mult(ref parent.Matrix, ref LocalMatrix, out Matrix);
            }
            else
            {
                Matrix = LocalMatrix;
            }

            Time.StopTimer("Transform.CalculateMatrix()");
        }

        public Transform Copy { get { return new Transform() { Position = Position, Rotation = Rotation, Scale = Scale }; } }
    }
}
