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

        public Extents(Shape startingShape)
        {
            AddShape(startingShape);
        }

        public Extents(List<Shape> shapes)
        {
            foreach (var shape in shapes)
            {
                AddShape(shape);
            }
        }

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
                    Max = new IntVector2(shape.Max);
                    Min = new IntVector2(shape.Min);

                }
                else
                {
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