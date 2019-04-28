using System.Collections.Generic;

namespace WorldObjects.Spaces.Geometry
{
    public class Shape
    {
        private readonly List<IntVector2> _vertices = new List<IntVector2>();
        public List<IntVector2> Vertices => new List<IntVector2>(_vertices);

        private readonly List<Segment> _segments = new List<Segment>();
        public List<Segment> Segments => new List<Segment>(_segments);

        public IntVector2 Max { get; private set; }
        public IntVector2 Min { get; private set; }

        public Shape(List<IntVector2> vertices)
        {
            if (vertices == null || vertices.Count < 2)
            {
                throw new System.InvalidOperationException($"Bounds must have at least 2 verticies.");
            }

            Max = new IntVector2(int.MinValue, int.MinValue);
            Min = new IntVector2(int.MaxValue, int.MaxValue);

            for (var vertexIndex = 0; vertexIndex < vertices.Count; vertexIndex++)
            {
                var vertex = new IntVector2(vertices[vertexIndex]);
                var nextVertex = new IntVector2(vertices.LoopedNext(vertexIndex));

                _vertices.Add(new IntVector2(vertex));
                _segments.Add(new Segment(vertex, nextVertex));

                if (vertex.X > Max.X) Max.X = vertex.X;
                if (vertex.Y > Max.Y) Max.Y = vertex.Y;
                if (vertex.X < Min.X) Min.X = vertex.X;
                if (vertex.Y < Min.Y) Min.Y = vertex.Y;
            }
        }

        // Because all of our extents are ordered clockwise around the shape,
        // we contain all points on or to the right of every segment
        public bool Contains(IntVector2 point)
        {
            foreach (var segment in _segments)
            {
                var relationship = RelationshipToSegment(point, segment.Start, segment.End);
                if (relationship == Relationships.Undefined || relationship == Relationships.Left)
                {
                    return false;
                }
            }

            return true;
        }

        public static Shape Intersect(Shape shapeOne, Shape shapeTwo)
        {
            var result = new List<IntVector2>();

            var currentShape = shapeOne;
            var otherShape = shapeTwo;
            var currentSegment = currentShape._segments[0];
            result.Add(currentSegment.Start);

            do
            {
                var anyIntersections = false;
                foreach (var otherSegment in otherShape._segments)
                {
                    var point = Segment.Intersect(currentSegment, otherSegment);

                    if (point != null)
                    {
                        result.Add(point);
                        result.Add(otherSegment.End);
                        currentSegment = otherSegment;
                        currentShape = otherShape;

                        anyIntersections = true;
                        break;
                    }
                }

                if (!anyIntersections)
                {
                    result.Add(currentSegment.End);
                    currentSegment = currentShape._segments.LoopedNext(currentSegment);
                }

            } while (result[0] != result[result.Count - 1]);

            var intersectedShape = new Shape(result);

            if (intersectedShape == shapeOne || intersectedShape == shapeTwo)
            {
                throw new System.Exception($"Shapes do not intersect.");
            }
            else return intersectedShape;
        }

        // http://www.eecs.umich.edu/courses/eecs380/HANDOUTS/PROJ2/InsidePoly.html
        private Relationships RelationshipToSegment(IntVector2 point, IntVector2 segmentStart, IntVector2 segmentEnd)
        {
            var result = ((point.Y - segmentStart.Y) * (segmentEnd.X - segmentStart.X)) - ((point.X - segmentStart.X) * (segmentEnd.Y - segmentStart.Y));

            if (result < 0) return Relationships.Right;
            else if (result > 0) return Relationships.Left;
            else if (result == 0) return Relationships.On;
            else return Relationships.Undefined;
        }

        public override bool Equals(object obj) =>
            obj is Shape shape && EqualityComparer<List<IntVector2>>.Default.Equals(_vertices, shape._vertices);

        public override int GetHashCode() => 59827589 + EqualityComparer<List<IntVector2>>.Default.GetHashCode(_vertices);

        private enum Relationships
        {
            Left,
            Right,
            On,
            Undefined
        }
    }
};