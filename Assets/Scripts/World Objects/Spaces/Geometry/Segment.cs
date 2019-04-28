namespace WorldObjects.Spaces.Geometry
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
            float a1 = segmentOne.End.Y - segmentOne.Start.Y;
            float b1 = segmentOne.End.X - segmentOne.Start.X;
            var c1 = a1 * segmentOne.Start.X + b1 * segmentOne.Start.Y;

            float a2 = segmentTwo.End.Y - segmentTwo.Start.Y;
            float b2 = segmentTwo.Start.X - segmentTwo.End.X;
            var c2 = a2 * segmentTwo.Start.X + b2 * segmentTwo.Start.Y;

            var delta = a1 * b2 - a2 * b1;
            //If lines are parallel, the result will be null
            return delta == 0 ? null : new IntVector2((b2 * c1 - b1 * c2) / delta,
                                                      (a1 * c2 - a2 * c1) / delta);
        }
    }
}