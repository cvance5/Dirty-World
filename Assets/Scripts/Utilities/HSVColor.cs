using UnityEngine;

public struct HSVColor
{
    public float h;
    public float s;
    public float v;
    public float a;

    public HSVColor(float h, float s, float v)
    {
        this.h = h;
        this.s = s;
        this.v = v;
        a = 1;
    }

    public HSVColor(float h, float s, float v, float a)
    {
        this.h = h;
        this.s = s;
        this.v = v;
        this.a = a;
    }

    public HSVColor(Color rgbColor)
    {
        Color.RGBToHSV(rgbColor, out h, out s, out v);
        a = rgbColor.a;
    }

    public HSVColor(HSVColor color)
    {
        h = color.h;
        s = color.s;
        v = color.v;
        a = color.a;
    }

    public static Color HSVToRBG(HSVColor color)
    {
        return Color.HSVToRGB(color.h, color.s, color.v);
    }
}
