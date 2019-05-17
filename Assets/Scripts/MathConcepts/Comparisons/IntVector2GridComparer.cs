using System.Collections.Generic;

namespace MathConcepts.Comparisons
{
    public class IntVector2GridComparer : Comparer<IntVector2>
    {
        public override int Compare(IntVector2 first, IntVector2 second)
        {
            if (first.X != second.X)
            {
                return first.X - second.X;
            }
            else return first.Y - second.Y;
        }
    }
}