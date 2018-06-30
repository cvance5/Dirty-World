using UnityEngine;

public static class ColorExtensions
{
    public static Color SwapRed(this Color source, float newRed)
    {
        return new Color(newRed, source.g, source.b, source.a);
    }
    public static Color SwapGreen(this Color source, float newGreen)
    {
        return new Color(source.r, newGreen, source.b, source.a);
    }
    public static Color SwapBlue(this Color source, float newBlue)
    {
        return new Color(source.r, source.g, newBlue, source.a);
    }
    public static Color SwapAlpha(this Color source, float newAlpha)
    {
        return new Color(source.r, source.g, source.b, newAlpha);
    }
}
