using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Hatzap.Utilities
{
    public static class MathHelper
    {
        public static double Lerp(double a, double b, double t)
        {
            return (1.0 - t) * a + t * b;
        }

        public static float Lerp(float a, float b, float t)
        {
            return (1.0f - t) * a + t * b;
        }

        public static Vector2 Lerp(Vector2 a, Vector2 b, float t)
        {
            return (1.0f - t) * a + t * b;
        }

        public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
        {
            return (1.0f - t) * a + t * b;
        }

        public static Vector4 Lerp(Vector4 a, Vector4 b, float t)
        {
            return (1.0f - t) * a + t * b;
        }
    }
}
