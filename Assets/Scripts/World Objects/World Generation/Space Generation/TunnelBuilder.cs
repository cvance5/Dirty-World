using MathConcepts;
using MathConcepts.Geometry;
using System.Collections.Generic;
using UnityEngine;

namespace WorldObjects.WorldGeneration.SpaceGeneration
{
    public class TunnelBuilder : SpaceBuilder
    {
        public override bool IsValid => _origin != null && _length > _minLength && _width > _minWidth;

        private int _length;
        private int _minLength = 1;
        private int _width;
        private int _minWidth = 1;

        private Quaternion _rotation;

        private IntVector2 _lengthwiseVector;
        private IntVector2 _widthwiseVector;

        public TunnelBuilder(ChunkBuilder chunkBuilder)
            : base(chunkBuilder)
        {
            _length = Chance.Range(4, 40);
            _width = Chance.Range(3, _length);

            var zRot = Chance.Range(0, 90);
            _rotation = Quaternion.Euler(0, 0, zRot);

            _origin = new IntVector2(Chance.Range(_chunkBuilder.BottomLeftCorner.X, _chunkBuilder.TopRightCorner.X),
                                     Chance.Range(_chunkBuilder.BottomLeftCorner.Y, _chunkBuilder.TopRightCorner.Y));

            Recalculate();
        }

        public TunnelBuilder SetLength(int length)
        {
            _length = length;

            Recalculate();

            return this;
        }

        public TunnelBuilder TrimToLength(int maximumLength)
        {
            if (_length > maximumLength)
            {
                _length = maximumLength;

                Recalculate();
            }

            return this;
        }

        public TunnelBuilder SetMinimumLength(int minLength)
        {
            _minLength = minLength;

            return this;
        }

        public TunnelBuilder SetWidth(int width)
        {
            if (_width != width)
            {
                _width = width;

                Recalculate();
            }

            return this;
        }

        public TunnelBuilder SetMinimumWidth(int minWidth)
        {
            _minWidth = minWidth;

            return this;
        }

        public TunnelBuilder SetRotation(float rotation)
        {
            var quatRotation = Quaternion.Euler(0, 0, rotation);

            if (_rotation != quatRotation)
            {
                _rotation = quatRotation;

                Recalculate();
            }

            return this;
        }

        public TunnelBuilder SetRotation(Quaternion rotation)
        {
            if (_rotation.eulerAngles.x != 0 ||
               _rotation.eulerAngles.y != 0)
            {
                throw new System.ArgumentOutOfRangeException($"Tunnels cannot rotate on any non-Z axis.");
            }
            else if (_rotation != rotation)
            {
                _rotation = rotation;

                Recalculate();
            }

            return this;
        }

        public TunnelBuilder SetOrigin(IntVector2 origin)
        {
            if (_origin != origin)
            {
                _origin = origin;

                Recalculate();
            }

            return this;
        }

        public override bool Contains(IntVector2 point) =>
            point.X >= _maximalValues[Directions.Left] &&
            point.X <= _maximalValues[Directions.Right] &&
            point.Y >= _maximalValues[Directions.Down] &&
            point.Y <= _maximalValues[Directions.Up];

        public override void Cut(IntVector2 direction, int limit)
        {
            var difference = DistanceFrom(direction, limit);

            if (difference > 0)
            {
                if (direction != Directions.Up &&
                   direction != Directions.Right &&
                   direction != Directions.Down &&
                   direction != Directions.Left)
                {
                    throw new System.ArgumentException($" Expected a cardinal direction.  Cannot operate on {direction}.");
                }

                var lengthSide = new Segment(_origin, _origin + _lengthwiseVector);
                foreach (var boundedDirection in _boundedDirections)
                {
                    lengthSide.Trim(boundedDirection.Key, boundedDirection.Value);
                }
                lengthSide = Segment.Shift(lengthSide, _widthwiseVector);
                foreach (var boundedDirection in _boundedDirections)
                {
                    lengthSide.Trim(boundedDirection.Key, boundedDirection.Value);
                }
                lengthSide = Segment.Shift(lengthSide, -_widthwiseVector);

                _length = (int)lengthSide.Length;
                _rotation = Quaternion.Euler(0, 0, lengthSide.Rotation);
                _origin = lengthSide.Start;

                var updatedWidthwiseVector = _rotation * new IntVector2(0, _width);
                var widthwiseVector = new IntVector2((int)updatedWidthwiseVector.x, (int)updatedWidthwiseVector.y);

                var widthSide = new Segment(_origin, _origin + widthwiseVector);
                foreach (var boundedDirection in _boundedDirections)
                {
                    widthSide.Trim(boundedDirection.Key, boundedDirection.Value);
                }
                widthSide = Segment.Shift(widthSide, _lengthwiseVector);
                foreach (var boundedDirection in _boundedDirections)
                {
                    widthSide.Trim(boundedDirection.Key, boundedDirection.Value);
                }
                widthSide = Segment.Shift(widthSide, -_lengthwiseVector);

                _width = (int)widthSide.Length;
                _origin = widthSide.Start;

                Recalculate();
            }
        }

        protected override Spaces.Space BuildRaw()
        {
            var extents = new Extents(new Shape(new List<IntVector2>()
            {
                _origin,
                _origin + _widthwiseVector,
                _origin + _widthwiseVector + _lengthwiseVector,
                _origin + _lengthwiseVector
            }));

            return new Spaces.Space($"Tunnel {Name}", extents);
        }

        protected override void Recalculate()
        {
            var vec1 = _rotation * new Vector2(_length, 0);
            var vec2 = _rotation * new Vector2(0, _width);
            _lengthwiseVector = new IntVector2((int)vec1.x, (int)vec1.y);
            _widthwiseVector = new IntVector2((int)vec2.x, (int)vec2.y);

            var segOne = new Segment(_origin, _origin + _widthwiseVector);
            var segTwo = Segment.Shift(segOne, _lengthwiseVector);

            _maximalValues[Directions.Up] = Mathf.Max(segOne.Start.Y, segOne.End.Y, segTwo.Start.Y, segTwo.End.Y);
            _maximalValues[Directions.Right] = Mathf.Max(segOne.Start.X, segOne.End.X, segTwo.Start.X, segTwo.End.X);
            _maximalValues[Directions.Down] = Mathf.Min(segOne.Start.Y, segOne.End.Y, segTwo.Start.Y, segTwo.End.Y);
            _maximalValues[Directions.Left] = Mathf.Min(segOne.Start.X, segOne.End.X, segTwo.Start.X, segTwo.End.X);

            OnSpaceBuilderChanged.Raise(this);
        }
    }
}