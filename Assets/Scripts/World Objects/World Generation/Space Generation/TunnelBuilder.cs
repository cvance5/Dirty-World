using MathConcepts;
using MathConcepts.Geometry;
using System.Collections.Generic;
using UnityEngine;

namespace WorldObjects.WorldGeneration.SpaceGeneration
{
    public class TunnelBuilder : SpaceBuilder
    {
        public override bool IsValid => Origin != null && Length > _minLength && Width > _minWidth;

        public int Length { get; private set; }
        private int _minLength = 1;
        public int Width { get; private set; }
        private int _minWidth = 1;

        public Quaternion Rotation { get; private set; }

        private IntVector2 _lengthwiseVector;
        private IntVector2 _widthwiseVector;

        public TunnelBuilder(ChunkBuilder chunkBuilder)
            : base(chunkBuilder)
        {
            Length = Chance.Range(4, 40);
            Width = Chance.Range(3, Length);

            var zRot = Chance.Range(0, 90);
            Rotation = Quaternion.Euler(0, 0, zRot);

            Origin = new IntVector2(Chance.Range(_chunkBuilder.BottomLeftCorner.X, _chunkBuilder.TopRightCorner.X),
                                     Chance.Range(_chunkBuilder.BottomLeftCorner.Y, _chunkBuilder.TopRightCorner.Y));

            Recalculate();
        }

        public TunnelBuilder SetLength(int length)
        {
            Length = length;

            Recalculate();

            return this;
        }

        public TunnelBuilder TrimToLength(int maximumLength)
        {
            if (Length > maximumLength)
            {
                Length = maximumLength;

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
            if (Width != width)
            {
                Width = width;

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

            if (Rotation != quatRotation)
            {
                Rotation = quatRotation;

                Recalculate();
            }

            return this;
        }

        public TunnelBuilder SetRotation(Quaternion rotation)
        {
            if (Rotation.eulerAngles.x != 0 ||
               Rotation.eulerAngles.y != 0)
            {
                throw new System.ArgumentOutOfRangeException($"Tunnels cannot rotate on any non-Z axis.");
            }
            else if (Rotation != rotation)
            {
                Rotation = rotation;

                Recalculate();
            }

            return this;
        }

        public TunnelBuilder SetOrigin(IntVector2 origin)
        {
            if (Origin != origin)
            {
                Origin = origin;

                Recalculate();
            }

            return this;
        }

        public override bool Contains(IntVector2 point) =>
            point.X >= _maximalValues[Directions.Left] &&
            point.X <= _maximalValues[Directions.Right] &&
            point.Y >= _maximalValues[Directions.Down] &&
            point.Y <= _maximalValues[Directions.Up];

        public override void Squash(IntVector2 direction, int limit)
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

                var lengthSide = new Segment(Origin, Origin + _lengthwiseVector);
                lengthSide.Trim(direction, limit);
                lengthSide = Segment.Shift(lengthSide, _widthwiseVector);
                lengthSide.Trim(direction, limit);
                lengthSide = Segment.Shift(lengthSide, -_widthwiseVector);
                Length = (int)lengthSide.Length;
                Rotation = Quaternion.Euler(0, 0, lengthSide.Rotation);
                Origin = lengthSide.Start;

                var updatedWidthwiseVector = Rotation * new IntVector2(0, Width);
                var widthwiseVector = new IntVector2((int)updatedWidthwiseVector.x, (int)updatedWidthwiseVector.y);

                var widthSide = new Segment(Origin, Origin + widthwiseVector);
                widthSide.Trim(direction, limit);
                widthSide = Segment.Shift(widthSide, _lengthwiseVector);
                widthSide.Trim(direction, limit);
                widthSide = Segment.Shift(widthSide, -_lengthwiseVector);
                Width = (int)widthSide.Length;
                Origin = widthSide.Start;

                Recalculate();
            }
        }

        protected override Spaces.Space BuildRaw()
        {
            var extents = new Extents(new Shape(new List<IntVector2>()
            {
                Origin,
                Origin + _widthwiseVector,
                Origin + _widthwiseVector + _lengthwiseVector,
                Origin + _lengthwiseVector
            }));

            return new Spaces.Space($"Tunnel {Name}", extents);
        }

        protected override void Recalculate()
        {
            var vec1 = Rotation * new Vector2(Length, 0);
            var vec2 = Rotation * new Vector2(0, Width);
            _lengthwiseVector = new IntVector2((int)vec1.x, (int)vec1.y);
            _widthwiseVector = new IntVector2((int)vec2.x, (int)vec2.y);

            var segOne = new Segment(Origin, Origin + _widthwiseVector);
            var segTwo = Segment.Shift(segOne, _lengthwiseVector);

            _maximalValues[Directions.Up] = Mathf.Max(segOne.Start.Y, segOne.End.Y, segTwo.Start.Y, segTwo.End.Y);
            _maximalValues[Directions.Right] = Mathf.Max(segOne.Start.X, segOne.End.X, segTwo.Start.X, segTwo.End.X);
            _maximalValues[Directions.Down] = Mathf.Min(segOne.Start.Y, segOne.End.Y, segTwo.Start.Y, segTwo.End.Y);
            _maximalValues[Directions.Left] = Mathf.Min(segOne.Start.X, segOne.End.X, segTwo.Start.X, segTwo.End.X);

            OnSpaceBuilderChanged.Raise(this);
        }
    }
}