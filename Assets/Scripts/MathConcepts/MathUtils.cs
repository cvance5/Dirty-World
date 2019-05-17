using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MathConcepts
{
    public static class MathUtils
    {
        public static double Sum(IEnumerable<double> nums)
        {
            double sum = 0;

            foreach (var num in nums)
            {
                sum += num;
            }

            return sum;
        }
        public static float Sum(IEnumerable<float> nums)
        {
            float sum = 0;

            foreach (var num in nums)
            {
                sum += num;
            }

            return sum;
        }
        public static decimal Sum(IEnumerable<decimal> nums)
        {
            decimal sum = 0;

            foreach (var num in nums)
            {
                sum += num;
            }

            return sum;
        }

        public static double Average(IEnumerable<double> nums) => Sum(nums) / nums.Count();
        public static decimal Average(IEnumerable<decimal> nums) => Sum(nums) / nums.Count();
        public static float Average(IEnumerable<float> nums) => Sum(nums) / nums.Count();

        public static float MapRange(float x, float minIn, float maxIn, float minOut, float maxOut)
        {
            return Mathf.Clamp((x - minIn) * (maxOut - minOut) / (maxIn - minIn) + minOut, minOut, maxOut);
        }

        public static int MapRange(int x, int minIn, int maxIn, int minOut, int maxOut)
        {
            return Mathf.RoundToInt(Mathf.Clamp((x - minIn) * (maxOut - minOut) / (float)(maxIn - minIn) + minOut, minOut, maxOut));
        }
    }
}