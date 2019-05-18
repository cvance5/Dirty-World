using System.Collections.Generic;
using Utilities.Debug;

namespace MathConcepts.Geometry
{
    public class Extents : IBoundary
    {
        private readonly List<Shape> _shapes = new List<Shape>();
        public List<Shape> Shapes => new List<Shape>(_shapes);

        public IntVector2 Max { get; private set; }
        public IntVector2 Min { get; private set; }
        public Shape Perimeter { get; private set; }

        public Extents() { }
        public Extents(Extents other) => AddExtents(other);

        public void AddExtents(Extents other)
        {
            foreach (var shape in other.Shapes)
            {
                AddShape(shape);
            }
        }
        public void AddShape(List<IntVector2> vertexes) => AddShape(new Shape(vertexes));
        private void AddShape(Shape shape)
        {
            if (_shapes.Contains(shape))
            {
                _log.Warning($"Adding duplicate shape to extents is wasteful.  Discarding.");
            }
            else
            {
                _shapes.Add(shape);

                if (_shapes.Count == 1)
                {
                    Perimeter = shape;
                    Max = shape.Max;
                    Min = shape.Min;

                }
                else
                {
                    Perimeter = Shape.Intersect(Perimeter, shape);
                    if (shape.Max.X > Max.X) Max.X = shape.Max.X;
                    if (shape.Max.Y > Max.Y) Max.Y = shape.Max.Y;
                    if (shape.Min.X < Min.X) Min.X = shape.Min.X;
                    if (shape.Min.Y < Min.Y) Min.Y = shape.Min.Y;
                }
            }
        }

        public bool Contains(IntVector2 point)
        {
            foreach (var shape in _shapes)
            {
                if (shape.Contains(point)) return true;
            }

            return false;
        }

        public override bool Equals(object obj) => obj is Extents extents &&
                   EqualityComparer<List<Shape>>.Default.Equals(_shapes, extents._shapes) &&
                   EqualityComparer<List<Shape>>.Default.Equals(Shapes, extents.Shapes);

        public override int GetHashCode()
        {
            var hashCode = -385129449;
            hashCode = hashCode * -1521134295 + EqualityComparer<List<Shape>>.Default.GetHashCode(_shapes);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<Shape>>.Default.GetHashCode(Shapes);
            return hashCode;
        }

        private static readonly Log _log = new Log("Extents");
    }
}