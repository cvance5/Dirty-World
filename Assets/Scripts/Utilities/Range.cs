public struct Range
{
    public int Min { get; private set; }
    public int Max { get; private set; }

    public Range(int min, int max)
    {
        if (min > max) throw new System.ArgumentOutOfRangeException("Min cannot be greater than max.");

        Min = min;
        Max = max;
    }

    public bool IsInRange(int value) => (value >= Min && value <= Max);
}