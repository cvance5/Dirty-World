using System.Collections.Generic;
using System.Linq;

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
}
