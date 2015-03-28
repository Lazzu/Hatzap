using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hatzap.Utilities
{
    interface IRandomProvider
    {
        uint Seed { get; set; }

        /// <summary>
        /// Returns the next random number.
        /// </summary>
        /// <returns>The next random number</returns>
        double NextDouble();

        /// <summary>
        /// Returns the next random number.
        /// </summary>
        /// <returns>The next random number</returns>
        int NextInt();

        /// <summary>
        /// Returns the next random number.
        /// </summary>
        /// <returns>The next random number</returns>
        uint NextUInt();

        /// <summary>
        /// Returns the next random number.
        /// </summary>
        /// <returns>The next random number</returns>
        float NextFloat();

        /// <summary>
        /// Returns the next random number.
        /// </summary>
        /// <returns>The next random number</returns>
        byte NextByte();

        /// <summary>
        /// Returns a random number in between the min and max values.
        /// </summary>
        /// <param name="min">The min value</param>
        /// <param name="max">The max value</param>
        /// <returns>The random number</returns>
        double Range(double min, double max);

        /// <summary>
        /// Returns a random number in between the min and max values.
        /// </summary>
        /// <param name="min">The min value</param>
        /// <param name="max">The max value</param>
        /// <returns>The random number</returns>
        float Range(float min, float max);

        /// <summary>
        /// Returns a random number in between the min and max values.
        /// </summary>
        /// <param name="min">The min value</param>
        /// <param name="max">The max value</param>
        /// <returns>The random number</returns>
        int Range(int min, int max);

        /// <summary>
        /// Returns a random number in between the min and max values.
        /// </summary>
        /// <param name="min">The min value</param>
        /// <param name="max">The max value</param>
        /// <returns>The random number</returns>
        uint Range(uint min, uint max);
    }
}
