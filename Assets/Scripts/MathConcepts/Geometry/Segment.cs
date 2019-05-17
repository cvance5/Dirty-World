using System.Collections.Generic;

namespace MathConcepts.Geometry
{
    public class Segment
    {
        public IntVector2 Start { get; }
        public IntVector2 End { get; }

        public Segment(IntVector2 start, IntVector2 end)
        {
            Start = start;
            End = end;
        }

        // https://stackoverflow.com/questions/4543506/algorithm-for-intersection-of-2-lines
        public static IntVector2 Intersect(Segment segmentOne, Segment segmentTwo)
        {

            // See if the lines will ever intersect
            float a1 = segmentOne.End.Y - segmentOne.Start.Y;
            float b1 = segmentOne.End.X - segmentOne.Start.X;
            var c1 = a1 * segmentOne.Start.X + b1 * segmentOne.Start.Y;

            float a2 = segmentTwo.End.Y - segmentTwo.Start.Y;
            float b2 = segmentTwo.Start.X - segmentTwo.End.X;
            var c2 = a2 * segmentTwo.Start.X + b2 * segmentTwo.Start.Y;

            var delta = a1 * b2 - a2 * b1;
            //If lines are parallel, the result will be null
            if (delta == 0)
            {
                return null;
            }
            else
            {
                var result = new IntVector2((b2 * c1 - b1 * c2) / delta, (a1 * c2 - a2 * c1) / delta);

                // Make sure they intersect between the start and end
                if (!IntVector2.IsBetween(segmentOne.Start, segmentOne.End, result) ||
                   !IntVector2.IsBetween(segmentTwo.Start, segmentTwo.End, result))
                {
                    return null;
                }
                else return result;
            }
        }

        public override bool Equals(object obj)
        {
            var segment = obj as Segment;
            return segment != null &&
                   EqualityComparer<IntVector2>.Default.Equals(Start, segment.Start) &&
                   EqualityComparer<IntVector2>.Default.Equals(End, segment.End);
        }

        public override int GetHashCode()
        {
            var hashCode = -1676728671;
            hashCode = hashCode * -1521134295 + EqualityComparer<IntVector2>.Default.GetHashCode(Start);
            hashCode = hashCode * -1521134295 + EqualityComparer<IntVector2>.Default.GetHashCode(End);
            return hashCode;
        }
    }
}