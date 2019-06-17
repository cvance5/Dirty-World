using System.Collections.Generic;
using UnityEngine;

namespace MathConcepts.Geometry
{
    public class Segment
    {
        public IntVector2 Start { get; }
        public IntVector2 End { get; }

        public IntVector2 Vector => End - Start;
        public float Length => Vector.Magnitude;
        public float Rotation => MathUtils.RadianToDegree(Mathf.Atan2(Vector.Y, Vector.X));

        public Segment(IntVector2 start, IntVector2 end)
        {
            Start = start;
            End = end;
        }

        public bool ContainsX(int xValue) =>
            (Start.X <= xValue && xValue <= End.X) ||
            (Start.X >= xValue && xValue >= End.X);

        public bool ContainsY(int YValue) =>
            (Start.Y <= YValue && YValue <= End.Y) ||
            (Start.Y >= YValue && YValue >= End.Y);

        public void Trim(IntVector2 direction, int newValue)
        {
            if (direction == Directions.Right || direction == Directions.Left)
            {
                if (ContainsX(newValue))
                {
                    // Anchor at whichever one point is less far in that direction
                    var anchorPosition = Start;
                    if ((direction == Directions.Right && End.X < Start.X) ||
                       (direction == Directions.Left && End.X > Start.X))
                    {
                        anchorPosition = End;
                    }

                    // Move the other guy
                    var movablePosition = anchorPosition == Start ? End : Start;

                    // Figure out % of change and apply same change to y
                    var percent = 1 - (newValue - (float)anchorPosition.X) / ((float)movablePosition.X - anchorPosition.X);

                    // This line is parallel with the cut point
                    if (float.IsNaN(percent)) return;

                    var newY = Mathf.Lerp(movablePosition.Y, anchorPosition.Y, percent);
                    var newYValue = Mathf.RoundToInt(newY);

                    // Calculate delta and set
                    movablePosition.X = newValue;
                    movablePosition.Y = newYValue;
                }
                // We don't contain this point, so collapse our line to it
                else
                {
                    Start.X = newValue;
                    End.X = Start.X;
                    End.Y = Start.Y;
                }
            }
            else if (direction == Directions.Up || direction == Directions.Down)
            {
                if (ContainsY(newValue))
                {
                    // Anchor at whichever one point is less far in that direction
                    var anchorPosition = Start;
                    if ((direction == Directions.Up && End.Y < Start.Y) ||
                       (direction == Directions.Down && End.Y > Start.Y))
                    {
                        anchorPosition = End;
                    }

                    // Move the other guy
                    var movablePosition = anchorPosition == Start ? End : Start;

                    // Figure out % of change and apply same change to y
                    var percent = 1 - (newValue - (float)anchorPosition.Y) / ((float)movablePosition.Y - anchorPosition.Y);

                    // This line is parallel with the cut point
                    if (float.IsNaN(percent)) return;

                    var newX = Mathf.Lerp(movablePosition.X, anchorPosition.X, percent);
                    var newXValue = Mathf.RoundToInt(newX);

                    // Calculate delta and set
                    movablePosition.X = newXValue;
                    movablePosition.Y = newValue;
                }
                // We don't contain this point, so collapse our line to it
                else
                {
                    Start.Y = newValue;
                    End.X = Start.X;
                    End.Y = Start.Y;
                }
            }
            else throw new System.ArgumentException($" Expected a cardinal direction.  Cannot operate on {direction}.");
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

        public static Segment Shift(Segment segment, IntVector2 shift) => new Segment(segment.Start + shift, segment.End + shift);

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