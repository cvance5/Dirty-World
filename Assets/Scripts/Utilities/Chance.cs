using System;

public static class Chance
{
    private static Random _rand = new Random();

    public static bool CoinFlip => (_rand.Next()%2) == 1;
}