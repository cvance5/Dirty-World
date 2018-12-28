using Items;
using UnityEngine;
using WorldObjects.Spaces;
using Space = WorldObjects.Spaces.Space;

namespace WorldObjects.WorldGeneration.SpaceGeneration
{
    public class TreasureRoomBuilder : SpaceBuilder
    {
        public override bool IsValid => _size >= _minimumSize && _treasure != null;

        private int _size;
        private int _minimumSize = 1;

        private IntVector2 _centerpoint;
        private Item[] _treasure;

        private IntVector2 _bottomLeftCorner;
        private IntVector2 _topRightCorner;

        public TreasureRoomBuilder(ChunkBuilder chunkBuilder)
            : base(chunkBuilder)
        {
            _size = Random.Range(2, 7);
            _centerpoint = new IntVector2(Random.Range(_chunkBuilder.BottomLeftCorner.X, _chunkBuilder.TopRightCorner.X),
                                          Random.Range(_chunkBuilder.BottomLeftCorner.Y, _chunkBuilder.TopRightCorner.Y));
            _treasure = null;

            Rebuild();
        }

        public override void Shift(IntVector2 shift)
        {
            _centerpoint += shift;

            Rebuild();
        }

        public TreasureRoomBuilder SetCenter(IntVector2 centerofRoom)
        {
            _centerpoint = centerofRoom;

            Rebuild();

            return this;
        }

        public TreasureRoomBuilder SetSize(int size)
        {
            _size = size;

            Rebuild();

            return this;
        }


        public TreasureRoomBuilder SetMinimumSize(int minimumSize)
        {
            _minimumSize = minimumSize;

            return this;
        }

        public TreasureRoomBuilder SetTreasure(params Item[] treasure)
        {
            _treasure = treasure;

            return this;
        }

        private void Rebuild()
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

        protected override Space BuildRaw() => new TreasureRoom(_bottomLeftCorner, _topRightCorner, _treasure);
    }
}