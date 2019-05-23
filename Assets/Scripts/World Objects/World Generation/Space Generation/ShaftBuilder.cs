using MathConcepts;
using MathConcepts.Geometry;
using System.Collections.Generic;
using UnityEngine;

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

        public ShaftBuilder(ChunkBuilder chunkBuilder)
            : base(chunkBuilder)
        {
            _width = Chance.Range(1, 10);
            _height = Chance.Range(_width + 1, 100);

            var startingPoint = new IntVector2(Chance.Range(chunkBuilder.BottomLeftCorner.X, chunkBuilder.TopRightCorner.X),
                                               Chance.Range(chunkBuilder.BottomLeftCorner.Y, chunkBuilder.TopRightCorner.Y));

            SetStartingPoint(startingPoint, Enum<ShaftAlignment>.Random);
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
                case ShaftAlignment.StartFromTop:
                    _top = startingPoint;
                    _origin = _top;
                    break;
                case ShaftAlignment.StartFromMiddle:
                    _middle = startingPoint;
                    _origin = _middle;
                    break;
                case ShaftAlignment.StartFromBottom:
                    _bottom = startingPoint;
                    _origin = _bottom;
                    break;
            }

            _alignment = alignment;

            Recalculate();
            return this;
        }

        public virtual ShaftBuilder SetWidth(int blocksWide)
        {
            _width = Mathf.Max(0, blocksWide);
            Recalculate();
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
            Recalculate();
            return this;
        }

        public ShaftBuilder SetMinimumHeight(int minBlocksHigh)
        {
            _minHeight = minBlocksHigh;
            return this;
        }

        public override bool Contains(IntVector2 position) =>
            position.X >= _bottom.X &&
            position.X <= _bottom.X + _width &&
            position.Y >= _bottom.Y &&
            position.Y <= _top.Y;

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

        protected override Spaces.Space BuildRaw()
        {
            var extents = new Extents(new Shape(new List<IntVector2>()
            {
                new IntVector2(_bottom),
                new IntVector2(_top),
                new IntVector2(_top.X + _width, _top.Y),
                new IntVector2(_bottom.X + _width, _bottom.Y)
            }));

            return new Spaces.Space($"Shaft {SpaceNamer.GetName()}", extents);
        }

        protected override void Recalculate()
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

            _maximalValues[Directions.Up] = _top.Y;
            _maximalValues[Directions.Right] = _top.X + _width;
            _maximalValues[Directions.Down] = _bottom.Y;
            _maximalValues[Directions.Left] = _bottom.X;

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