using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public Matrix4 InvProjection = Matrix4.Identity;
        public Matrix4 InvView = Matrix4.Identity;
        public Matrix4 InvViewRotation = Matrix4.Identity;

        public Camera()
        {
            Up = Vector3.UnitY;
        }

        public void Perspective(float width, float height, float near, float far)
        {
            Projection = Matrix4.CreatePerspectiveFieldOfView((float)(Math.PI / 2), width / height, near, far);
        }

        public virtual void Update(float deltaTime)
        {
            View = Matrix4.LookAt(Position, Target, Up);

            Direction = Position - Target;
            Direction.Normalize();

            VPMatrix = View * Projection;

            InvView = View.Inverted();

            Quaternion cameraRotation = View.ExtractRotation();
            cameraRotation.Invert();

            InvViewRotation = Matrix4.CreateFromQuaternion(cameraRotation);
        }

        public void SetAsCurrent()
        {
            Current = this;
        }

        public static Camera Current = null;
    }
}
