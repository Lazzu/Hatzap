﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using OpenTK;

namespace Hatzap.Utilities
{
    /*	 
	 * 
	 * Tämä on copypaste MonoGamen BoundingSphere luokasta, joka on MIT eli sillä voi käytännössä tehdä mitä huvittaa
	 * 
	 * */



    public class BoundingSphere : IEquatable<BoundingSphere>, IBounder
    {
        #region IBounder implementation

        public ContainmentType Inside(BoundingBox box)
        {
            return box.Contains(this);
        }

        public ContainmentType Inside(BoundingSphere sphere)
        {
            return sphere.Contains(this);
        }

        public ContainmentType Inside(BoundingFrustum frustum)
        {
            return frustum.Contains(this);
        }

        #endregion

        #region Public Fields

        public Vector3 Center;

        public float Radius;

        #endregion Public Fields


        #region Constructors

        protected BoundingSphere()
        {

        }

        public BoundingSphere(Vector3 center, float radius)
        {
            this.Center = center;
            this.Radius = radius;
        }

        #endregion Constructors


        #region Public Methods

        public BoundingSphere Transform(Matrix4 matrix)
        {
            BoundingSphere sphere = new BoundingSphere();
            sphere.Center = Vector3.Transform(this.Center, matrix);
            sphere.Radius = this.Radius * ((float)Math.Sqrt((double)Math.Max(((matrix.M11 * matrix.M11) + (matrix.M12 * matrix.M12)) + (matrix.M13 * matrix.M13), Math.Max(((matrix.M21 * matrix.M21) + (matrix.M22 * matrix.M22)) + (matrix.M23 * matrix.M23), ((matrix.M31 * matrix.M31) + (matrix.M32 * matrix.M32)) + (matrix.M33 * matrix.M33)))));
            return sphere;
        }

        public void Transform(ref Matrix4 matrix, out BoundingSphere result)
        {
            result = this;
            result.Center = Vector3.Transform(this.Center, matrix);
            result.Radius = this.Radius * ((float)Math.Sqrt((double)Math.Max(((matrix.M11 * matrix.M11) + (matrix.M12 * matrix.M12)) + (matrix.M13 * matrix.M13), Math.Max(((matrix.M21 * matrix.M21) + (matrix.M22 * matrix.M22)) + (matrix.M23 * matrix.M23), ((matrix.M31 * matrix.M31) + (matrix.M32 * matrix.M32)) + (matrix.M33 * matrix.M33)))));
        }

        public ContainmentType Contains(BoundingBox box)
        {
            //check if all corner is in sphere
            bool inside = true;
            foreach (Vector3 corner in box.GetCorners())
            {
                if (this.Contains(corner) == ContainmentType.Disjoint)
                {
                    inside = false;
                    break;
                }
            }

            if (inside)
                return ContainmentType.Contains;

            //check if the distance from sphere center to cube face < radius
            double dmin = 0;

            if (Center.X < box.Min.X)
                dmin += (Center.X - box.Min.X) * (Center.X - box.Min.X);

            else if (Center.X > box.Max.X)
                dmin += (Center.X - box.Max.X) * (Center.X - box.Max.X);

            if (Center.Y < box.Min.Y)
                dmin += (Center.Y - box.Min.Y) * (Center.Y - box.Min.Y);

            else if (Center.Y > box.Max.Y)
                dmin += (Center.Y - box.Max.Y) * (Center.Y - box.Max.Y);

            if (Center.Z < box.Min.Z)
                dmin += (Center.Z - box.Min.Z) * (Center.Z - box.Min.Z);

            else if (Center.Z > box.Max.Z)
                dmin += (Center.Z - box.Max.Z) * (Center.Z - box.Max.Z);

            if (dmin <= Radius * Radius)
                return ContainmentType.Intersects;

            //else disjoint
            return ContainmentType.Disjoint;

        }

        public void Contains(ref BoundingBox box, out ContainmentType result)
        {
            result = this.Contains(box);
        }

        public ContainmentType Contains(BoundingFrustum frustum)
        {
            //check if all corner is in sphere
            bool inside = true;

            Vector3[] corners = frustum.GetCorners();
            foreach (Vector3 corner in corners)
            {
                if (this.Contains(corner) == ContainmentType.Disjoint)
                {
                    inside = false;
                    break;
                }
            }
            if (inside)
                return ContainmentType.Contains;

            //check if the distance from sphere center to frustrum face < radius
            double dmin = 0;
            //TODO : calcul dmin

            if (dmin <= Radius * Radius)
                return ContainmentType.Intersects;

            //else disjoint
            return ContainmentType.Disjoint;
        }

        public ContainmentType Contains(BoundingSphere sphere)
        {
            float val = (sphere.Center - Center).Length;

            if (val > sphere.Radius + Radius)
                return ContainmentType.Disjoint;

            else if (val <= Radius - sphere.Radius)
                return ContainmentType.Contains;

            else
                return ContainmentType.Intersects;
        }

        public void Contains(ref BoundingSphere sphere, out ContainmentType result)
        {
            result = Contains(sphere);
        }

        public ContainmentType Contains(Vector3 point)
        {
            float distance = (point - Center).Length;

            if (distance > this.Radius)
                return ContainmentType.Disjoint;

            else if (distance < this.Radius)
                return ContainmentType.Contains;

            return ContainmentType.Intersects;
        }

        public void Contains(ref Vector3 point, out ContainmentType result)
        {
            result = Contains(point);
        }

        public static BoundingSphere CreateFromBoundingBox(BoundingBox box)
        {
            // Find the center of the box.
            Vector3 center = new Vector3((box.Min.X + box.Max.X) / 2.0f,
                (box.Min.Y + box.Max.Y) / 2.0f,
                (box.Min.Z + box.Max.Z) / 2.0f);

            // Find the distance between the center and one of the corners of the box.
            float radius = (center - box.Max).Length;

            return new BoundingSphere(center, radius);
        }

        public static void CreateFromBoundingBox(ref BoundingBox box, out BoundingSphere result)
        {
            result = CreateFromBoundingBox(box);
        }

        public static BoundingSphere CreateFromFrustum(BoundingFrustum frustum)
        {
            return BoundingSphere.CreateFromPoints(frustum.GetCorners());
        }

        public static BoundingSphere CreateFromPoints(IEnumerable<Vector3> points)
        {
            if (points == null)
                throw new ArgumentNullException("points");

            float radius = 0;
            Vector3 center = new Vector3();
            // First, we'll find the center of gravity for the point 'cloud'.
            int num_points = 0; // The number of points (there MUST be a better way to get this instead of counting the number of points one by one?)

            foreach (Vector3 v in points)
            {
                center += v;    // If we actually knew the number of points, we'd get better accuracy by adding v / num_points.
                ++num_points;
            }

            center /= (float)num_points;

            // Calculate the radius of the needed sphere (it equals the distance between the center and the point further away).
            foreach (Vector3 v in points)
            {
                float distance = ((v - center)).Length;

                if (distance > radius)
                    radius = distance;
            }

            return new BoundingSphere(center, radius);
        }

        public static BoundingSphere CreateMerged(BoundingSphere original, BoundingSphere additional)
        {
            Vector3 ocenterToaCenter = Vector3.Subtract(additional.Center, original.Center);
            float distance = ocenterToaCenter.Length;
            if (distance <= original.Radius + additional.Radius)//intersect
            {
                if (distance <= original.Radius - additional.Radius)//original contain additional
                    return original;
                if (distance <= additional.Radius - original.Radius)//additional contain original
                    return additional;
            }

            //else find center of new sphere and radius
            float leftRadius = Math.Max(original.Radius - distance, additional.Radius);
            float Rightradius = Math.Max(original.Radius + distance, additional.Radius);
            ocenterToaCenter = ocenterToaCenter + (((leftRadius - Rightradius) / (2 * ocenterToaCenter.Length)) * ocenterToaCenter);//oCenterToResultCenter

            BoundingSphere result = new BoundingSphere();
            result.Center = original.Center + ocenterToaCenter;
            result.Radius = (leftRadius + Rightradius) / 2;
            return result;
        }

        public static void CreateMerged(ref BoundingSphere original, ref BoundingSphere additional, out BoundingSphere result)
        {
            result = BoundingSphere.CreateMerged(original, additional);
        }

        public bool Equals(BoundingSphere other)
        {
            return this.Center == other.Center && this.Radius == other.Radius;
        }

        public override bool Equals(object obj)
        {
            if (obj is BoundingSphere)
                return this.Equals((BoundingSphere)obj);

            return false;
        }

        public override int GetHashCode()
        {
            return this.Center.GetHashCode() + this.Radius.GetHashCode();
        }

        public bool Intersects(BoundingBox box)
        {
            return box.Intersects(this);
        }

        public void Intersects(ref BoundingBox box, out bool result)
        {
            result = Intersects(box);
        }

        /*		
        public bool Intersects(BoundingFrustum frustum)
        {
            if (frustum == null)
                throw new NullReferenceException();

            throw new NotImplementedException();
        }
        */

        public bool Intersects(BoundingSphere sphere)
        {
            float val = (sphere.Center - Center).Length;
            if (val > sphere.Radius + Radius)
                return false;
            return true;
        }

        public void Intersects(ref BoundingSphere sphere, out bool result)
        {
            result = Intersects(sphere);
        }

        public PlaneIntersectionType Intersects(Plane plane)
        {
            var result = default(PlaneIntersectionType);
            // TODO: we might want to inline this for performance reasons
            this.Intersects(ref plane, out result);
            return result;
        }

        public void Intersects(ref Plane plane, out PlaneIntersectionType result)
        {
            float distance = default(float);
            // TODO: we might want to inline this for performance reasons
            Vector3.Dot(ref plane.Normal, ref this.Center, out distance);
            distance += plane.D;
            if (distance > this.Radius)
                result = PlaneIntersectionType.Front;
            else if (distance < -this.Radius)
                result = PlaneIntersectionType.Back;
            else
                result = PlaneIntersectionType.Intersecting;
        }

        public float? Intersects(Ray ray)
        {
            return ray.Intersects(this);
        }

        public void Intersects(ref Ray ray, out float? result)
        {
            result = Intersects(ray);
        }

        public static bool operator ==(BoundingSphere a, BoundingSphere b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(BoundingSphere a, BoundingSphere b)
        {
            return !a.Equals(b);
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "{{Center:{0} Radius:{1}}}", this.Center.ToString(), this.Radius.ToString());
        }

        #endregion Public Methods
    }
}