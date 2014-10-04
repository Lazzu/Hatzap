using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hatzap.Utilities;

namespace Hatzap
{
    public class Camera
    {
        public Vector3 Position { get; set; }
        public Vector3 Target { get; set; }
        public Vector3 Up { get; set; }

        public Vector3 Direction = -Vector3.UnitZ;

        public Matrix4 Projection = Matrix4.Identity;
        public Matrix4 View = Matrix4.Identity;
        public Matrix4 VPMatrix = Matrix4.Identity;
        public Matrix4 MVPMatrix = Matrix4.Identity;
        public Matrix3 NormalMatrix = Matrix3.Identity;

        public Matrix4 InvProjection = Matrix4.Identity;
        public Matrix4 InvView = Matrix4.Identity;
        public Matrix4 InvVPMatrix = Matrix4.Identity;

        public BoundingFrustum Frustum;

        GameWindow gameWindow;

        public Camera(GameWindow gw)
        {
            Up = Vector3.UnitY;

            gameWindow = gw;
        }

        public void Perspective(float width, float height, float fov, float near, float far)
        {
            float fovy = (float)(Math.PI / 180) * fov;
            Projection = Matrix4.CreatePerspectiveFieldOfView(fovy, width / height, near, far);
            InvProjection = Projection.Inverted();
        }

        public virtual void Update(float deltaTime)
        {
            View = Matrix4.LookAt(Position, Target, Up);
            InvView = View.Inverted();

            Direction = Position - Target;
            Direction.Normalize();

            VPMatrix = View * Projection;
            InvVPMatrix = VPMatrix.Inverted();

            var rotation = VPMatrix.ExtractRotation();
            NormalMatrix = Matrix3.CreateFromQuaternion(rotation);

            Frustum = new BoundingFrustum(VPMatrix);
        }

        public void GetModelViewProjection(ref Matrix4 modelMatrix, out Matrix4 mvp)
        {
            Matrix4.Mult(ref modelMatrix, ref VPMatrix, out mvp);
        }

        // Hold data here, don't allocate every time it's needed.
        Matrix4 mMV = Matrix4.Identity;

        public void GetNormalMatrix(ref Matrix4 modelMatrix, out Matrix3 mN)
        {
            Matrix4.Mult(ref modelMatrix, ref View, out mMV);
            var rotation = mMV.ExtractRotation(); // How to get rid of this allocation?
            Matrix3.CreateFromQuaternion(ref rotation, out mN);
        }

        public void SetAsCurrent()
        {
            Current = this;
        }

        /// <summary>
        /// Returns a world-space ray from a window point.
        /// </summary>
        /// <param name="point">The window-space point.</param>
        /// <returns>A ray in world-space.</returns>
        public Ray GetRayFromWindowPoint(Vector2 point)
        {
            var near = UnProject(WindowPointToScreenPoint(point, 0));
            var far = UnProject(WindowPointToScreenPoint(point, 1));
            
            var d = far - near;
            d.Normalize();

            return new Ray(near, d);
        }
        
        public Vector3 UnProject(Vector3 p)
        {
            var tmp = Vector4.Transform(new Vector4(p, 1), InvVPMatrix);
            var scalar = 1 / tmp.W;
            return tmp.Xyz * scalar;
        }

        /// <summary>
        /// Convert window coordinates to screen-space coordinates.
        /// </summary>
        /// <param name="point">The coordinates in window-space</param>
        /// <param name="z">Optional z coordinate. Zero by default.</param>
        /// <returns>A point in screen-space.</returns>
        public Vector3 WindowPointToScreenPoint(Vector2 point, float z = 0)
        {
            return new Vector3(point.X / (float)gameWindow.Width * 2 - 1, -(point.Y / (float)gameWindow.Height * 2 - 1), z);
        }

        /// <summary>
        /// Convert the camera to a ray.
        /// </summary>
        /// <returns>A ray that goes in the direction the camera looks at from the camera position.</returns>
        public Ray ToRay()
        {
            return new Ray(Position, Direction);
        }

        public static Camera Current = null;
    }
}
