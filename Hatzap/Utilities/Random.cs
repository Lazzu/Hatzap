using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hatzap.Utilities
{
    /// <summary>
    /// A stable pseudo-random number generator. Unlike System.Random, this 
    /// Will produce the same random numbers with same seeds on all platforms.
    /// </summary>
    public class Random : IRandomProvider
    {
        private uint m_z = 1337;
        private uint m_w = 7331;

        public uint Seed
        {
            get
            {
                return m_z;
            }
            set
            {
                m_z = value;
            }
        }

        public uint Seed2
        {
            get
            {
                return m_w;
            }
            set
            {
                m_w = value;
            }
        }

        /// <summary>
        /// Returns a random number between 0 and 1. It never returns exactly 1.
        /// </summary>
        /// <returns>A random number between 0 and 1</returns>
        public double NextDouble()
        {
            // 0 <= u < 2^32
            uint u = NextUInt();
            // The magic number below is 1/(2^32 + 2).
            // The result is strictly between 0 and 1, never 0 or 1.
            double d = (u + 1.0) * 2.328306435454494e-10;

            // Make it between -1 and 1
            d *= 2;
            d -= 1;

            // Return absolute value (0 - 1, never 1)
            return Math.Abs(d);
        }

        /// <summary>
        /// Returns a random integer number between int.MinValue and int.MaxValue
        /// </summary>
        /// <returns>A random integer number between int.MinValue and int.MaxValue</returns>
        public int NextInt()
        {
            return (int)(NextUInt() - (uint)int.MaxValue);
        }

        /// <summary>
        /// Returns a random unsigned ingeger
        /// </summary>
        /// <returns>A random unsigned integer</returns>
        public uint NextUInt()
        {
            m_z = 36969 * (m_z & 65535) + (m_z >> 16);
            m_w = 18000 * (m_w & 65535) + (m_w >> 16);
            return (m_z << 16) + m_w;
        }

        /// <summary>
        /// Returns a float number between 0 and 1
        /// </summary>
        /// <returns>A float number between 0 and 1</returns>
        public float NextFloat()
        {
            return (float)NextDouble();
        }

        /// <summary>
        /// Returns a random byte in between 0 and 255
        /// </summary>
        /// <returns></returns>
        public byte NextByte()
        {
            return (byte)Math.Abs((ushort)(NextDouble() * 256));
        }

        /// <summary>
        /// Returns a random integer in between a range
        /// </summary>
        /// <param name="min">The minimum value</param>
        /// <param name="max">The maximum value</param>
        /// <returns>A random integer</returns>
        public int Range(int min, int max)
        {
            int tmp = Math.Min(min, max);
            max = Math.Max(min, max);
            min = tmp;

            return (int)(min + (max - min) * NextDouble());
        }

        /// <summary>
        /// Returns a random unsigned integer in between a range
        /// </summary>
        /// <param name="min">The minimum value</param>
        /// <param name="max">The maximum value</param>
        /// <returns>A random unsigned integer</returns>
        public uint Range(uint min, uint max)
        {
            uint tmp = Math.Min(min, max);
            max = Math.Max(min, max);
            min = tmp;

            return (uint)(min + (max - min) * NextDouble());
        }

        /// <summary>
        /// Returns a random double in between a range
        /// </summary>
        /// <param name="min">The minimum value</param>
        /// <param name="max">The maximum value</param>
        /// <returns>A random double</returns>
        public double Range(double min, double max)
        {
            double tmp = Math.Min(min, max);
            max = Math.Max(min, max);
            min = tmp;

            return min + (max - min) * NextDouble();
        }

        /// <summary>
        /// Returns a random float in between a range
        /// </summary>
        /// <param name="min">The minimum value</param>
        /// <param name="max">The maximum value</param>
        /// <returns>A random float</returns>
        public float Range(float min, float max)
        {
            float tmp = Math.Min(min, max);
            max = Math.Max(min, max);
            min = tmp;

            return (float)(min + (max - min) * NextDouble());
        }
    }
}
