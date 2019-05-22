using MathConcepts;
using MathConcepts.Geometry;
using System.Collections.Generic;
using UnityEngine;
using Space = WorldObjects.Spaces.Space;

namespace WorldObjects.WorldGeneration.SpaceGeneration
{
    public class RoomBuilder : SpaceBuilder
    {
        public override bool IsValid => _size >= _minimumSize;

        protected int _size;
        protected int _minimumSize = 1;

        protected IntVector2 _centerpoint;

        protected IntVector2 _bottomLeftCorner;
        protected IntVector2 _topRightCorner;

        public RoomBuilder(ChunkBuilder chunkBuilder)
            : base(chunkBuilder)
        {
            _size = Chance.Range(2, 7);
            _centerpoint = new IntVector2(Chance.Range(_chunkBuilder.BottomLeftCorner.X, _chunkBuilder.TopRightCorner.X),
                                          Chance.Range(_chunkBuilder.BottomLeftCorner.Y, _chunkBuilder.TopRightCorner.Y));

            Recalculate();
        }

        public RoomBuilder(RoomBuilder roomBuilder)
            : base(roomBuilder)
        {
            _size = roomBuilder._size;
            _minimumSize = roomBuilder._minimumSize;
            _centerpoint = roomBuilder._centerpoint;

            _modifiersApplied.AddRange(roomBuilder._modifiersApplied);

            Recalculate();
        }

        public override void Shift(IntVector2 shift) => SetCenter(_centerpoint + shift);

        public RoomBuilder SetCenter(IntVector2 centerofRoom)
        {
            _centerpoint = centerofRoom;

            Recalculate();

            return this;
        }

        public RoomBuilder SetSize(int size)
        {
            _size = Mathf.Max(0, size);

            Recalculate();

            return this;
        }

        public RoomBuilder SetMinimumSize(int minimumSize)
        {
            _minimumSize = minimumSize;

            return this;
        }

        public override bool Contains(IntVector2 position) =>
            position.X >= _bottomLeftCorner.X &&
            position.X <= _topRightCorner.X &&
            position.Y >= _bottomLeftCorner.Y &&
            position.Y <= _topRightCorner.Y;

        public override void Clamp(IntVector2 direction, int amount)
        {
            var difference = PassesBy(direction, amount);

            if (difference > 0)
            {
                Align(direction, amount);
            }
        }

        public override void Cut(IntVector2 direction, int amount)
        {
            var difference = PassesBy(direction, amount);

            if (difference > 0)
            {
                SetSize(_size - difference);
                Align(direction, amount);
            }
        }

        protected override Space BuildRaw()
        {
            var extents = new Extents(new Shape(new List<IntVector2>()
            {
                new IntVector2(_bottomLeftCorner),
                new IntVector2(_bottomLeftCorner.X, _topRightCorner.Y),
                new IntVector2(_topRightCorner),
                new IntVector2(_topRightCorner.X, _bottomLeftCorner.Y)
            }));

            return new Space($"Room {SpaceNamer.GetName()}", extents);
        }

        protected override void Recalculate()
        {
            // Odd numbers don't divide by 2 and will result in weird values,
            // so this ensures the size is always right
            var halfSize = _size / 2;
            _bottomLeftCorner = new IntVector2(_centerpoint.X - halfSize, _centerpoint.Y - halfSize);
            _topRightCorner = new IntVector2(_centerpoint.X + _size - halfSize, _centerpoint.Y + _size - halfSize);

            _maximalValues[Directions.Up] = _topRightCorner.Y;
            _maximalValues[Directions.Right] = _topRightCorner.X;
            _maximalValues[Directions.Down] = _bottomLeftCorner.Y;
            _maximalValues[Directions.Left] = _bottomLeftCorner.X;

            OnSpaceBuilderChanged.Raise(this);
        }
    }
}