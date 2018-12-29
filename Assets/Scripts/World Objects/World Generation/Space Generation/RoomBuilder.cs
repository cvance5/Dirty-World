using UnityEngine;
using WorldObjects.Spaces;
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
            _size = Random.Range(2, 7);
            _centerpoint = new IntVector2(Random.Range(_chunkBuilder.BottomLeftCorner.X, _chunkBuilder.TopRightCorner.X),
                                          Random.Range(_chunkBuilder.BottomLeftCorner.Y, _chunkBuilder.TopRightCorner.Y));

            Rebuild();
        }

        public RoomBuilder(RoomBuilder roomBuilder)
            : base(roomBuilder)
        {
            _size = roomBuilder._size;
            _minimumSize = roomBuilder._minimumSize;
            _centerpoint = roomBuilder._centerpoint;

            Rebuild();
        }

        public override void Shift(IntVector2 shift) => SetCenter(_centerpoint + shift);

        public RoomBuilder SetCenter(IntVector2 centerofRoom)
        {
            _centerpoint = centerofRoom;

            Rebuild();

            return this;
        }

        public RoomBuilder SetSize(int size)
        {
            _size = size;

            Rebuild();

            return this;
        }

        public RoomBuilder SetMinimumSize(int minimumSize)
        {
            _minimumSize = minimumSize;

            return this;
        }

        protected void Rebuild()
        {
            _bottomLeftCorner = new IntVector2(_centerpoint.X - _size, _centerpoint.Y - _size);
            _topRightCorner = new IntVector2(_centerpoint.X + _size, _centerpoint.Y + _size);
        }

        public override int PassesBy(IntVector2 direction, int amount)
        {
            var difference = 0;

            if (direction == Directions.Up)
            {
                difference = _topRightCorner.Y - amount;
            }
            else if (direction == Directions.Right)
            {
                difference = (_topRightCorner.X) - amount;
            }
            else if (direction == Directions.Down)
            {
                difference = amount - _bottomLeftCorner.Y;
            }
            else if (direction == Directions.Left)
            {
                difference = amount - _bottomLeftCorner.X;
            }
            else throw new System.ArgumentException($" Expected a cardinal direction.  Cannot operate on {direction}.");

            return Mathf.Max(0, difference);
        }

        public override bool Contains(IntVector2 position) =>
            position.X >= _bottomLeftCorner.X &&
            position.X <= _topRightCorner.X &&
            position.Y >= _bottomLeftCorner.Y &&
            position.Y <= _topRightCorner.Y;

        public override IntVector2 GetRandomPoint() =>
            new IntVector2(Random.Range(_bottomLeftCorner.X, _topRightCorner.X + 1),
                           Random.Range(_bottomLeftCorner.Y, _topRightCorner.Y + 1));

        public override int GetMaximalValue(IntVector2 direction)
        {
            if (direction == Directions.Up) return _topRightCorner.Y;
            else if (direction == Directions.Right) return _topRightCorner.X;
            else if (direction == Directions.Down) return _bottomLeftCorner.Y;
            else if (direction == Directions.Left) return _bottomLeftCorner.X;
            else throw new System.ArgumentException($" Expected a cardinal direction.  Cannot operate on {direction}.");
        }

        public override SpaceBuilder Align(IntVector2 direction, int amount)
        {
            if (direction == Directions.Up)
            {
                _centerpoint.Y = amount - _size;
            }
            else if (direction == Directions.Right)
            {
                _centerpoint.X = amount - _size;
            }
            else if (direction == Directions.Down)
            {
                _centerpoint.Y = amount + _size;
            }
            else if (direction == Directions.Left)
            {
                _centerpoint.X = amount + _size;
            }
            else throw new System.ArgumentException($" Expected a cardinal direction.  Cannot operate on {direction}.");

            Rebuild();

            return this;
        }

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
            }
        }

        protected override Space BuildRaw() => new Room(_bottomLeftCorner, _topRightCorner);
    }
}