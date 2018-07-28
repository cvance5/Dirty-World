using System;
using WorldObjects.Spaces;

namespace WorldObjects.WorldGeneration.SpaceGeneration
{
    public class CorridorBuilder : SpaceBuilder
    {
        private IntVector2 _leftEnd;
        private IntVector2 _center;
        private IntVector2 _rightEnd;
        private CorridorAlignment _alignment;

        private int _height;
        private int _length;

        private bool _isHazardous;

        public CorridorBuilder(ChunkBuilder containingChunk)
            : base(containingChunk)
        {
            var rand = new Random();

            _height = rand.Next(1, 10);
            _length = rand.Next(1, 100);

            IntVector2 startingPoint = new IntVector2(rand.Next(containingChunk.BottomLeft.X, containingChunk.TopRight.X), 
                                                      rand.Next(containingChunk.BottomLeft.Y, containingChunk.TopRight.Y));

            SetStartingPoint(startingPoint, Enum<CorridorAlignment>.Random);

            _isHazardous = Chance.OneIn(5);
        }

        public CorridorBuilder SetStartingPoint(IntVector2 startingPoint, CorridorAlignment alignment)
        {
            switch (alignment)
            {
                case CorridorAlignment.StartFromLeft: _leftEnd = startingPoint; break;
                case CorridorAlignment.StartFromCenter: _center = startingPoint; break;
                case CorridorAlignment.StartFromRight: _rightEnd = startingPoint; break;
            }

            _alignment = alignment;
            FindAllPoints();

            return this;
        }

        public CorridorBuilder SetLength(int blocksLong)
        {
            _length = blocksLong - 1;
            FindAllPoints();
            return this;
        }

        public CorridorBuilder SetHeight(int blockHigh)
        {
            _height = blockHigh - 1;
            FindAllPoints();
            return this;
        }

        public CorridorBuilder SetHazards(bool hasHazards)
        {
            _isHazardous = hasHazards;
            return this;
        }

        public override SpaceBuilder Clamp(IntVector2 direction, int amount)
        {
            if (direction == Directions.Up)
            {
                if (_leftEnd.Y + _height > amount)
                {
                    var difference = (_leftEnd.Y + _height) - amount;
                    _leftEnd.Y -= difference;
                    _center.Y -= difference;
                    _rightEnd.Y -= difference;
                }

            }
            else if (direction == Directions.Right)
            {
                _rightEnd.X = Math.Min(_rightEnd.X, amount);
                _alignment = CorridorAlignment.StartFromRight; // We have to enforce this boundary
            }
            else if (direction == Directions.Down)
            {
                if (_leftEnd.Y < amount)
                {
                    _leftEnd.Y = amount;
                    _center.Y = amount;
                    _rightEnd.Y = amount;
                }
            }
            else if (direction == Directions.Left)
            {
                _leftEnd.X = Math.Max(_leftEnd.X, amount);
                _alignment = CorridorAlignment.StartFromLeft; // We have to enforce this boundary
            }
            else throw new ArgumentException($" Expected a cardinal direction.  Cannot operate on {direction}.");

            FindAllPoints();
            return this;
        }

        public override Space Build() => new Corridor(_leftEnd, new IntVector2(_rightEnd.X, _rightEnd.Y + _height), _isHazardous);

        private void FindAllPoints()
        {
            switch (_alignment)
            {
                case CorridorAlignment.StartFromLeft:
                    if (_leftEnd == null) throw new InvalidOperationException("Corridor builder has not been assigned a left position.");
                    else
                    {
                        _center = new IntVector2(_leftEnd.X + (_length / 2), _leftEnd.Y);
                        _rightEnd = new IntVector2(_leftEnd.X + _length, _leftEnd.Y);
                    }
                    break;
                case CorridorAlignment.StartFromCenter:
                    if (_center == null) throw new InvalidOperationException("Corridor builder has not been assigned a central position.");
                    else
                    {
                        _leftEnd = new IntVector2(_center.X - (_length / 2), _center.Y);
                        _rightEnd = new IntVector2(_center.X + (_length / 2), _center.Y);
                    }
                    break;
                case CorridorAlignment.StartFromRight:
                    if (_rightEnd == null) throw new InvalidOperationException("Corridor builder has not been assigned a right position.");
                    else
                    {
                        _leftEnd = new IntVector2(_rightEnd.X - _length, _rightEnd.Y);
                        _center = new IntVector2(_rightEnd.X - (_length / 2), _rightEnd.Y);
                    }
                    break;
            }
        }

        public enum CorridorAlignment
        {
            StartFromLeft,
            StartFromCenter,
            StartFromRight
        }
    }
}

