using System.Collections.Generic;

namespace MathConcepts.Geometry
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
                throw new System.ArgumentException($"Bounds must have at least 2 verticies.");
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

        public static bool DoesIntersect(Shape shapeOne, Shape shapeTwo)
        {
            foreach (var vertex in shapeOne.Vertices)
            {
                if (shapeTwo.Contains(vertex)) return true;
            }

            return false;
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

        public override bool Equals(object obj)
        {
            if (obj is Shape shape)
            {
                if (_vertices.Count != shape._vertices.Count) return false;

                for (var vertexIndex = 0; vertexIndex < _vertices.Count; vertexIndex++)
                {
                    if (_vertices[vertexIndex] != shape._vertices[vertexIndex]) return false;
                }
                return true;
            }
            else return false;
        }

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