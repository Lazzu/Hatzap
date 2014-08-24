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

        public Matrix4 Projection = Matrix4.Identity;
        public Matrix4 View = Matrix4.Identity;
        public Matrix4 VPMatrix = Matrix4.Identity;
        public Matrix4 MVPMatrix = Matrix4.Identity;

        public Camera()
        {
            Up = Vector3.UnitY;
        }

        public virtual void Update(float deltaTime)
        {
            View = Matrix4.LookAt(Position, Target, Up);
            VPMatrix = Projection * View;
        }

        public void SetAsCurrent()
        {
            Current = this;
        }

        public static Camera Current = null;
    }
}
