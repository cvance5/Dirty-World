using UnityEngine;
using WorldObjects.Spaces;

namespace WorldObjects.WorldGeneration.SpaceGeneration
{
    public class ShaftBuilder : SpaceBuilder
    {
        public override bool IsValid => _height >= _minHeight && _width >= _minWidth;

        protected IntVector2 _top;
        protected IntVector2 _middle;
        protected IntVector2 _bottom;
        private ShaftAlignment _alignment;

        protected int _height;
        private int _minHeight = 1;
        protected int _width;
        private int _minWidth = 1;

        protected bool _isUncapped;

        public ShaftBuilder(ChunkBuilder chunkBuilder)
            : base(chunkBuilder)
        {
            _width = Chance.Range(1, 10);
            _height = Chance.Range(_width + 1, 100);

            var startingPoint = new IntVector2(Chance.Range(chunkBuilder.BottomLeftCorner.X, chunkBuilder.TopRightCorner.X),
                                               Chance.Range(chunkBuilder.BottomLeftCorner.Y, chunkBuilder.TopRightCorner.Y));

            SetStartingPoint(startingPoint, Enum<ShaftAlignment>.Random);
        }

        public override void Shift(IntVector2 shift)
        {
            _top += shift;
            _middle += shift;
            _bottom += shift;

            OnSpaceBuilderChanged.Raise(this);
        }

        public ShaftBuilder SetStartingPoint(IntVector2 startingPoint, IntVector2 direction)
        {
            if (direction == Directions.Up)
            {
                return SetStartingPoint(startingPoint, ShaftAlignment.StartFromBottom);
            }
            else if (direction == Directions.Down)
            {
                return SetStartingPoint(startingPoint, ShaftAlignment.StartFromTop);
            }
            else return SetStartingPoint(startingPoint, ShaftAlignment.StartFromMiddle);
        }

        public ShaftBuilder SetStartingPoint(IntVector2 startingPoint, ShaftAlignment alignment)
        {
            switch (alignment)
            {
                case ShaftAlignment.StartFromTop: _top = startingPoint; break;
                case ShaftAlignment.StartFromMiddle: _middle = startingPoint; break;
                case ShaftAlignment.StartFromBottom: _bottom = startingPoint; break;
            }

            _alignment = alignment;

            Rebuild();
            return this;
        }

        public ShaftBuilder SetWidth(int blocksWide)
        {
            _width = Mathf.Max(0, blocksWide);
            Rebuild();
            return this;
        }

        public ShaftBuilder SetMinimumWidth(int minBlocksWide)
        {
            _minWidth = minBlocksWide;
            return this;
        }

        public virtual ShaftBuilder SetHeight(int blockHigh)
        {
            _height = Mathf.Max(0, blockHigh);
            Rebuild();
            return this;
        }

        public ShaftBuilder SetMinimumHeight(int minBlocksHigh)
        {
            _minHeight = minBlocksHigh;
            return this;
        }

        public ShaftBuilder SetUncapped(bool isUncapped)
        {
            _isUncapped = isUncapped;
            return this;
        }

        public override int PassesBy(IntVector2 direction, int amount)
        {
            var difference = 0;

            if (direction == Directions.Up)
            {
                difference = _top.Y - amount;
            }
            else if (direction == Directions.Right)
            {
                difference = (_top.X + _width) - amount;
            }
            else if (direction == Directions.Down)
            {
                difference = amount - _bottom.Y;
            }
            else if (direction == Directions.Left)
            {
                difference = amount - _bottom.X;
            }
            else throw new System.ArgumentException($" Expected a cardinal direction.  Cannot operate on {direction}.");

            return Mathf.Max(0, difference);
        }

        public override bool Contains(IntVector2 position) =>
            position.X >= _bottom.X &&
            position.X <= _bottom.X + _width &&
            position.Y >= _bottom.Y &&
            position.Y <= _top.Y;

        public override IntVector2 GetRandomPoint() =>
            new IntVector2(Chance.Range(_bottom.X, _bottom.X + _width + 1),
                           Chance.Range(_bottom.Y, _top.Y + 1));

        public override int GetMaximalValue(IntVector2 direction)
        {
            if (direction == Directions.Up) return _top.Y;
            else if (direction == Directions.Right) return _top.X + _width;
            else if (direction == Directions.Down) return _bottom.Y;
            else if (direction == Directions.Left) return _top.X;
            else throw new System.ArgumentException($" Expected a cardinal direction.  Cannot operate on {direction}.");
        }

        public override SpaceBuilder Align(IntVector2 direction, int amount)
        {
            if (direction == Directions.Up)
            {
                _top.Y = amount;
                _alignment = ShaftAlignment.StartFromTop; // We have to enforce this boundary
                _isUncapped = true; // We shouldn't spawn blocks here, as it may be clamped by another space or chunk
            }
            else if (direction == Directions.Right)
            {
                _bottom.X = amount - _width;
                _middle.X = amount - _width;
                _top.X = amount - _width;
            }
            else if (direction == Directions.Down)
            {
                _bottom.Y = amount;
                _alignment = ShaftAlignment.StartFromBottom; // We have to enforce this boundary
            }
            else if (direction == Directions.Left)
            {
                _bottom.X = amount;
                _middle.X = amount;
                _top.X = amount;
            }

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
                if (direction == Directions.Up)
                {
                    SetHeight(_height - difference);
                }
                else if (direction == Directions.Right)
                {
                    SetWidth(_width - difference);
                }
                else if (direction == Directions.Down)
                {
                    SetHeight(_height - difference);
                }
                else if (direction == Directions.Left)
                {
                    SetWidth(_width - difference);
                    Clamp(Directions.Left, amount); // We have to shift the amount to the right, since we originate left
                }
                else throw new System.ArgumentException($" Expected a cardinal direction.  Cannot operate on {direction}.");
            }
        }

        protected override Spaces.Space BuildRaw() => new Shaft(_bottom, new IntVector2(_top.X + _width, _top.Y), _isUncapped);

        protected virtual void Rebuild()
        {
            switch (_alignment)
            {
                case ShaftAlignment.StartFromTop:
                    if (_top == null) throw new System.InvalidOperationException("Shaft builder has not been assigned a top position.");
                    else
                    {
                        _middle = new IntVector2(_top.X, _top.Y - (_height / 2));
                        _bottom = new IntVector2(_top.X, _top.Y - _height);
                    }
                    break;
                case ShaftAlignment.StartFromMiddle:
                    if (_middle == null) throw new System.InvalidOperationException("Shaft builder has not been assigned a middle position.");
                    else
                    {
                        _top = new IntVector2(_middle.X, _middle.Y + (_height / 2));
                        _bottom = new IntVector2(_middle.X, _middle.Y - (_height / 2));
                    }
                    break;
                case ShaftAlignment.StartFromBottom:
                    if (_bottom == null) throw new System.InvalidOperationException("Shaft builder has not been assigned a bottom position.");
                    else
                    {
                        _middle = new IntVector2(_bottom.X, _bottom.Y + (_height / 2));
                        _top = new IntVector2(_bottom.X, _bottom.Y + _height);
                    }
                    break;
            }

            OnSpaceBuilderChanged.Raise(this);
        }

        public enum ShaftAlignment
        {
            StartFromTop,
            StartFromMiddle,
            StartFromBottom
        }
    }
}