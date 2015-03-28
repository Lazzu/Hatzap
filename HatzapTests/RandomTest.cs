using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HatzapTests
{
    [TestClass]
    public class RandomTest
    {
        // Generated with 1234 as Seed and 4321 as Seed2
        readonly int[] preGeneratedInts = new int[] {
            -1631269807,
            -1832403549,
            311215806,
            -524419802,
            -986091482,
            -1231655144,
            3219454,
            -2037232748,
            -213133126,
            -1320590190,
        };

        readonly double[] preGeneratedDoubles = new double[] {
            0.759619198385803,
            0.853279395562932,
            0.14492115231933,
            0.244202000440936,
            0.459184629163153,
            0.573534120072828,
            0.00149917462770865,
            0.94866042400307,
            0.099247846240528,
            0.614947728060676,
        };

        [TestMethod]
        [TestCategory("Random numbers")]
        public void TestRandomInts()
        {
            var random = new Hatzap.Utilities.Random()
            {
                Seed = 1234,
                Seed2 = 4321
            };

            var random2 = new Hatzap.Utilities.Random()
            {
                Seed = 1234,
                Seed2 = 4321
            };

            for (int i = 0; i < preGeneratedInts.Length; i++)
            {
                var n1 = random.NextInt();
                Assert.AreEqual(preGeneratedInts[i], n1, "The given integers should match because they were generated with the same seeds");

                var n2 = random2.NextInt();
                Assert.AreEqual(n1, n2, "The given integers should match because they were generated with the same seeds");
            }
        }

        [TestMethod]
        [TestCategory("Random numbers")]
        public void TestRandomDoubles()
        {
            var random = new Hatzap.Utilities.Random()
            {
                Seed = 1234,
                Seed2 = 4321
            };

            var random2 = new Hatzap.Utilities.Random()
            {
                Seed = 1234,
                Seed2 = 4321
            };
            
            for (int i = 0; i < preGeneratedDoubles.Length; i++)
            {
                var d1 = random.NextDouble();

                Debug.WriteLine(Math.Abs(preGeneratedDoubles[i] - d1));

                Assert.AreEqual(preGeneratedDoubles[i], d1, 0.0000000000001, "The given doubles should match because they were generated with the same seeds");

                var d2 = random2.NextDouble();

                Assert.AreEqual(d1, d2, 0.0000000000001, "The given doubles should match because they were generated with the same seeds");
            }
        }
    }
}
