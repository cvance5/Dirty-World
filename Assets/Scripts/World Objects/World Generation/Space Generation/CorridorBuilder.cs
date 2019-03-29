using System.Collections.Generic;
using UnityEngine;
using WorldObjects.Spaces;
using WorldObjects.WorldGeneration.EnemyGeneration;

namespace WorldObjects.WorldGeneration.SpaceGeneration
{
    public class CorridorBuilder : SpaceBuilder
    {
        public override bool IsValid => _height >= _minHeight && _length >= _minLength;

        private IntVector2 _leftEnd;
        private IntVector2 _center;
        private IntVector2 _rightEnd;
        private CorridorAlignment _alignment;

        private int _height;
        private int _minHeight = 1;
        private int _length;
        private int _minLength = 1;

        private int _extraRiskPoints;
        private bool _allowEnemies = true;

        public CorridorBuilder(ChunkBuilder chunkBuilder)
            : base(chunkBuilder)
        {
            _height = Chance.Range(1, 10);
            _length = Chance.Range(_length + 1, 100);

            var startingPoint = new IntVector2(Chance.Range(_chunkBuilder.BottomLeftCorner.X, _chunkBuilder.TopRightCorner.X),
                                               Chance.Range(_chunkBuilder.BottomLeftCorner.Y, _chunkBuilder.TopRightCorner.Y));

            SetStartingPoint(startingPoint, Enum<CorridorAlignment>.Random);
        }

        public override void Shift(IntVector2 shift)
        {
            _leftEnd += shift;
            _center += shift;
            _rightEnd += shift;

            OnSpaceBuilderChanged.Raise(this);
        }

        public CorridorBuilder SetStartingPoint(IntVector2 startingPoint, IntVector2 direction)
        {
            if (direction == Directions.Left)
            {
                return SetStartingPoint(startingPoint, CorridorAlignment.StartFromRight);
            }
            else if (direction == Directions.Right)
            {
                return SetStartingPoint(startingPoint, CorridorAlignment.StartFromLeft);
            }
            else return SetStartingPoint(startingPoint, CorridorAlignment.StartFromCenter);
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
            Rebuild();

            return this;
        }

        public CorridorBuilder SetLength(int blocksLong)
        {
            _length = blocksLong - 1;
            _length = Mathf.Max(0, _length);
            Rebuild();
            return this;
        }

        public CorridorBuilder SetMinimumLength(int minimumBlocksLong)
        {
            _minLength = minimumBlocksLong - 1;
            return this;
        }

        public CorridorBuilder SetHeight(int blocksHigh)
        {
            _height = blocksHigh - 1;
            _height = Mathf.Max(0, _height);
            Rebuild();
            return this;
        }

        public CorridorBuilder SetMinimumHeight(int minimumBlocksHigh)
        {
            _minHeight = minimumBlocksHigh - 1;
            return this;
        }

        public CorridorBuilder SetExtraRiskPoints(int riskPoints)
        {
            _extraRiskPoints = riskPoints;
            return this;
        }

        public CorridorBuilder SetAllowEnemies(bool allowEnemies)
        {
            _allowEnemies = allowEnemies;
            return this;
        }

        public override int PassesBy(IntVector2 direction, int amount)
        {
            if (direction == Directions.Up)
            {
                return (_leftEnd.Y + _height) - amount;
            }
            else if (direction == Directions.Right)
            {
                return _rightEnd.X - amount;
            }
            else if (direction == Directions.Down)
            {
                return amount - _rightEnd.Y;
            }
            else if (direction == Directions.Left)
            {
                return amount - _leftEnd.X;
            }
            else throw new System.ArgumentException($" Expected a cardinal direction.  Cannot operate on {direction}.");
        }

        public override bool Contains(IntVector2 position) =>
            position.X >= _leftEnd.X &&
            position.Y >= _leftEnd.Y &&
            position.X <= _rightEnd.X &&
            position.Y <= _leftEnd.Y + _height;

        public override IntVector2 GetRandomPoint() => 
            new IntVector2(Chance.Range(_leftEnd.X, _rightEnd.X + 1),
                           Chance.Range(_leftEnd.Y, _leftEnd.Y + _height + 1));

        public override int GetMaximalValue(IntVector2 direction)
        {
            if (direction == Directions.Up) return _leftEnd.Y + _height;
            else if (direction == Directions.Right) return _rightEnd.X;
            else if (direction == Directions.Down) return _leftEnd.Y;
            else if (direction == Directions.Left) return _leftEnd.X;
            else throw new System.ArgumentException($" Expected a cardinal direction.  Cannot operate on {direction}.");
        }

        public override SpaceBuilder Align(IntVector2 direction, int amount)
        {
            if (direction == Directions.Up)
            {
                _leftEnd.Y = amount - _height;
                _center.Y = amount - _height;
                _rightEnd.Y = amount - _height;
            }
            else if (direction == Directions.Right)
            {
                _rightEnd.X = amount;
                _alignment = CorridorAlignment.StartFromRight; // We have to enforce this boundary
            }
            else if (direction == Directions.Down)
            {
                _leftEnd.Y = amount;
                _center.Y = amount;
                _rightEnd.Y = amount;
            }
            else if (direction == Directions.Left)
            {
                _leftEnd.X = Mathf.Max(_leftEnd.X, amount);
                _alignment = CorridorAlignment.StartFromLeft; // We have to enforce this boundary
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
                if (direction == Directions.Up)
                {
                    SetHeight(_height - difference);
                }
                else if (direction == Directions.Right)
                {
                    SetLength(_length - difference);
                }
                else if (direction == Directions.Down)
                {
                    SetHeight(_height - difference);
                    Clamp(Directions.Down, amount); // We have to shift the amount up, since we originate down
                }
                else if (direction == Directions.Left)
                {
                    SetLength(_length - difference);
                }
                else throw new System.ArgumentException($" Expected a cardinal direction.  Cannot operate on {direction}.");
            }
        }

        protected override Spaces.Space BuildRaw()
        {
            var corridor = new Corridor(_leftEnd, new IntVector2(_rightEnd.X, _rightEnd.Y + _height));
            corridor.AddEnemySpawns(GenerateContainedEnemies());
            return corridor;
        }

        private List<EnemySpawn> GenerateContainedEnemies()
        {
            var containedEnemies = new List<EnemySpawn>();

            if (_allowEnemies)
            {
                var riskPoints = EnemyPicker.DetermineRiskPoints(_chunkBuilder.Depth, _chunkBuilder.Remoteness);
                riskPoints += _extraRiskPoints;

                var enemies = EnemyPicker.RequestEnemies(riskPoints, new EnemyRequestCriteria()
                {
                    HeightsAllowed = new Range(0, _height),
                    LengthsAllowed = new Range(0, _length)
                });

                foreach (var enemy in enemies)
                {
                    var xPos = Chance.Range(_leftEnd.X, _rightEnd.X);
                    var position = new IntVector2(xPos, _leftEnd.Y);

                    containedEnemies.Add(new EnemySpawn(position, enemy));
                }
            }

            return containedEnemies;
        }

        private void Rebuild()
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

            OnSpaceBuilderChanged.Raise(this);
        }

        public enum CorridorAlignment
        {
            StartFromLeft,
            StartFromCenter,
            StartFromRight
        }
    }
}

