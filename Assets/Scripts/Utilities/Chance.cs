using System;
using System.Collections.Generic;
using System.Linq;

public static class Chance
{
    private static Random _rand = new Random();

    public static bool CoinFlip => OneIn(2);

    public static double Percent => _rand.NextDouble();

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