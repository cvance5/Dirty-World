using System;
using System.Collections.Generic;
using System.Linq;

public static class Chance
{
    private static Random _rand = new Random();

    public static bool CoinFlip => OneIn(2);

    public static bool OneIn(int x) => _rand.Next(x - 1) == 0;

    public static List<int> ExclusiveRandomOrder(int numNumbers)
    {
        List<int> random = new List<int>();
        for (int i = 0; i < numNumbers; i++)
        {
            random.Add(i);
        }

        return random.OrderBy(num => _rand.Next()).ToList();
    }
}