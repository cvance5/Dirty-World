using System.Collections.Generic;
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

        public CorridorBuilder(IntVector2 startingPoint, CorridorAlignment alignment)
        {
            switch (alignment)
            {
                case CorridorAlignment.StartFromLeft: _leftEnd = startingPoint; break;
                case CorridorAlignment.StartFromCenter: _center = startingPoint; break;
                case CorridorAlignment.StartFromRight: _rightEnd = startingPoint; break;
            }

            _alignment = alignment;
        }

        public CorridorBuilder SetLength(int blocksLong)
        {
            _length = blocksLong - 1;
            return this;
        }

        public CorridorBuilder SetHeight(int blockHigh)
        {
            _height = blockHigh - 1;
            return this;
        }

        public override Space Build()
        {
            FindAllPoints();
            
            return new Corridor(_leftEnd, new IntVector2(_rightEnd.X, _rightEnd.Y + _height));
        }

        private void FindAllPoints()
        {
            switch (_alignment)
            {
                case CorridorAlignment.StartFromLeft:
                    if (_leftEnd == null) throw new System.InvalidOperationException("Corridor builder has not been assigned a left position.");
                    else
                    {
                        _center = new IntVector2(_leftEnd.X + (_length / 2), _leftEnd.Y);
                        _rightEnd = new IntVector2(_leftEnd.X + _length, _leftEnd.Y);
                    }
                    break;
                case CorridorAlignment.StartFromCenter:
                    if (_center == null) throw new System.InvalidOperationException("Corridor builder has not been assigned a central position.");
                    else
                    {
                        _leftEnd = new IntVector2(_center.X - (_length / 2), _center.Y);
                        _rightEnd = new IntVector2(_center.X + (_length / 2), _center.Y);
                    }
                    break;
                case CorridorAlignment.StartFromRight:
                    if (_rightEnd == null) throw new System.InvalidOperationException("Corridor builder has not been assigned a right position.");
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

