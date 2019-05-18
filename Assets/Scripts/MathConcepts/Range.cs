using System;

namespace MathConcepts
{
    public struct Range
    {
        public int Min { get; set; }
        public int Max { get; set; }

        public int Size { get; }
        public float Center { get; }

        public Range(int min, int max)
        {
            if (min > max) throw new ArgumentOutOfRangeException("Min cannot be greater than max.");

            Min = min;
            Max = max;

            Size = Max - Min;
            Center = Min + (Size / 2);
        }

        public bool IsInRange(int value) => (value >= Min && value <= Max);

        public int DistanceFromCenter(int value) => (int)Math.Abs(Center - value);
    }
}