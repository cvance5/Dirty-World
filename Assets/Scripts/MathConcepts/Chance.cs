using System;
using System.Collections.Generic;
using System.Linq;

namespace MathConcepts
{
    public static class Chance
    {
        private static Random _rand = new Random();

        public static void Seed(int seed) => _rand = new Random(seed);

        public static bool CoinFlip => OneIn(2);

        public static double Percent => _rand.NextDouble();

        public static int RandomSignFlip(int value) => CoinFlip ? value : value *= -1;

        public static bool OneIn(int x)
        {
            var rand = _rand.Next(x);
            return rand == 0;
        }

        public static bool ChanceOf(float percent)
        {
            var rand = _rand.NextDouble();
            return rand < percent;
        }

        public static bool ChanceOf(int percent)
        {
            var rand = _rand.Next(100);
            return rand < percent;
        }

        /// <summary>
        /// https://stackoverflow.com/questions/3365337/best-way-to-generate-a-random-float-in-c-sharp
        /// </summary>
        /// <returns></returns>
        public static float NextFloat()
        {
            var mantissa = (_rand.NextDouble() * 2.0) - 1.0;
            var exponent = Math.Pow(2.0, _rand.Next(-126, 128));
            return (float)(mantissa * exponent);
        }

        public static int Range(Range range) => Range(range.Min, range.Max);

        /// <summary>
        /// Pass through to a Random.Next(min, max)
        /// </summary>
        /// <param name="min">Inclusive min</param>
        /// <param name="max">Exclusive max</param>
        /// <returns></returns>
        public static int Range(int min, int max) => _rand.Next(min, max);
        public static double Range(float min, float max)
        {
            var next = _rand.NextDouble();
            return min + (next * (max - min));
        }

        public static List<int> ExclusiveRandomOrder(int numNumbers)
        {
            var random = new List<int>();
            for (var i = 0; i < numNumbers; i++)
            {
                random.Add(i);
            }

            return random.OrderBy(num => _rand.Next()).ToList();
        }
    }
}