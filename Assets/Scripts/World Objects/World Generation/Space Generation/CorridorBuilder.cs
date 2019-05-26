using MathConcepts;
using MathConcepts.Geometry;
using System;
using System.Collections.Generic;
using UnityEngine;
using WorldObjects.Spaces;
using WorldObjects.WorldGeneration.EnemyGeneration;

namespace WorldObjects.WorldGeneration.SpaceGeneration
{
    public class CorridorBuilder : SpaceBuilder
    {
        public override bool IsValid => _origin != null && _height >= _minHeight && _length >= _minLength;

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
                case CorridorAlignment.StartFromLeft:
                    _leftEnd = startingPoint;
                    _origin = _leftEnd;
                    break;
                case CorridorAlignment.StartFromCenter:
                    _center = startingPoint;
                    _origin = _center;
                    break;
                case CorridorAlignment.StartFromRight:
                    _rightEnd = startingPoint;
                    _origin = _rightEnd;
                    break;
            }

            _alignment = alignment;
            Recalculate();

            return this;
        }

        public CorridorBuilder SetLength(int blocksLong)
        {
            _length = blocksLong;
            _length = Mathf.Max(0, _length);
            Recalculate();
            return this;
        }

        public CorridorBuilder SetMinimumLength(int minimumBlocksLong)
        {
            _minLength = minimumBlocksLong;
            return this;
        }

        public CorridorBuilder SetHeight(int blocksHigh)
        {
            _height = blocksHigh;
            _height = Mathf.Max(0, _height);
            Recalculate();
            return this;
        }

        public CorridorBuilder SetMinimumHeight(int minimumBlocksHigh)
        {
            _minHeight = minimumBlocksHigh;
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

        public override bool Contains(IntVector2 position) =>
            position.X >= _leftEnd.X &&
            position.Y >= _leftEnd.Y &&
            position.X <= _rightEnd.X &&
            position.Y <= _leftEnd.Y + _height;

        public override void Cut(IntVector2 direction, int amount)
        {
            var difference = DistanceFrom(direction, amount);

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
                else throw new ArgumentException($" Expected a cardinal direction.  Cannot operate on {direction}.");
            }
        }

        protected override Spaces.Space BuildRaw()
        {
            var extents = new Extents(new Shape(new List<IntVector2>()
            {
                new IntVector2(_leftEnd),
                new IntVector2(_leftEnd.X, _leftEnd.Y   + _height),
                new IntVector2(_rightEnd.X, _rightEnd.Y + _height),
                new IntVector2(_rightEnd)
            }));

            return new Spaces.Space($"Corridor {SpaceNamer.GetName()}", extents);
        }

        protected override void Recalculate()
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

            _maximalValues[Directions.Up] = _leftEnd.Y + _height;
            _maximalValues[Directions.Right] = _rightEnd.X;
            _maximalValues[Directions.Down] = _rightEnd.Y;
            _maximalValues[Directions.Left] = _leftEnd.X;

            OnSpaceBuilderChanged.Raise(this);
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

        public enum CorridorAlignment
        {
            StartFromLeft,
            StartFromCenter,
            StartFromRight
        }
    }
}

