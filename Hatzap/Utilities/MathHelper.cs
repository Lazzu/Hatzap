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
        public static void CalculateColorMultiplier(ref Vector4 targetColor, ref Vector4 originalColor, out Vector4 outColor)
        {
            outColor = new Vector4(targetColor.X / originalColor.X, targetColor.Y / originalColor.Y, targetColor.Z / originalColor.Z, targetColor.W / originalColor.W);
        }

        public static Vector4 CalculateColorMultiplier(Vector4 targetColor, Vector4 originalColor)
        {
            Vector4 outColor;
            CalculateColorMultiplier(ref targetColor, ref originalColor, out outColor);
            return outColor;
        }

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
