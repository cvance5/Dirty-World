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

        protected IntVector2 _bottomLeftCorner;
        protected IntVector2 _topRightCorner;

        public RoomBuilder(ChunkBuilder chunkBuilder)
            : base(chunkBuilder)
        {
            _size = Chance.Range(2, 7);
            _origin = new IntVector2(Chance.Range(_chunkBuilder.BottomLeftCorner.X, _chunkBuilder.TopRightCorner.X),
                                          Chance.Range(_chunkBuilder.BottomLeftCorner.Y, _chunkBuilder.TopRightCorner.Y));

            Recalculate();
        }

        public RoomBuilder(RoomBuilder roomBuilder)
            : base(roomBuilder)
        {
            _size = roomBuilder._size;
            _minimumSize = roomBuilder._minimumSize;
            _origin = roomBuilder._origin;

            _modifiersApplied.AddRange(roomBuilder._modifiersApplied);

            Recalculate();
        }

        public RoomBuilder SetCenter(IntVector2 centerofRoom)
        {
            _origin = centerofRoom;

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
            _bottomLeftCorner = new IntVector2(_origin.X - halfSize, _origin.Y - halfSize);
            _topRightCorner = new IntVector2(_origin.X + _size - halfSize, _origin.Y + _size - halfSize);

            _maximalValues[Directions.Up] = _topRightCorner.Y;
            _maximalValues[Directions.Right] = _topRightCorner.X;
            _maximalValues[Directions.Down] = _bottomLeftCorner.Y;
            _maximalValues[Directions.Left] = _bottomLeftCorner.X;

            OnSpaceBuilderChanged.Raise(this);
        }
    }
}