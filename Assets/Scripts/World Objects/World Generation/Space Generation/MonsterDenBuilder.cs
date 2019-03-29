using System.Collections.Generic;
using UnityEngine;
using WorldObjects.Spaces;
using WorldObjects.WorldGeneration.EnemyGeneration;

namespace WorldObjects.WorldGeneration.SpaceGeneration
{
    public class MonsterDenBuilder : SpaceBuilder
    {
        public override bool IsValid => _radius > 0;

        private IntVector2 _centerpoint;
        private int _radius;

        private int _extraRiskPoints;
        private bool _allowEnemies = true;

        public MonsterDenBuilder(ChunkBuilder chunkBuilder)
            : base(chunkBuilder)
        {
            _centerpoint = new IntVector2(Chance.Range(_chunkBuilder.BottomLeftCorner.X, _chunkBuilder.TopRightCorner.X),
                                          Chance.Range(_chunkBuilder.BottomLeftCorner.Y, _chunkBuilder.TopRightCorner.Y));

            _radius = Chance.Range(8, 20);
        }

        public override void Shift(IntVector2 shift) => _centerpoint += shift;

        public MonsterDenBuilder SetCenterpoint(IntVector2 centerpoint)
        {
            _centerpoint = centerpoint;
            OnSpaceBuilderChanged.Raise(this);
            return this;
        }

        public MonsterDenBuilder SetRadius(int radius)
        {
            _radius = radius;
            _radius = Mathf.Max(0, _radius);
            if (_radius == 0) SetAllowEnemies(false);
            OnSpaceBuilderChanged.Raise(this);
            return this;
        }

        public MonsterDenBuilder SetAllowEnemies(bool allowEnemies)
        {
            _allowEnemies = allowEnemies;
            return this;
        }

        public MonsterDenBuilder SetExtraRiskPoints(int riskPoints)
        {
            _extraRiskPoints = riskPoints;
            return this;
        }

        protected override Spaces.Space BuildRaw()
        {
            var monsterDen = new MonsterDen(_centerpoint, _radius);
            monsterDen.AddEnemySpawns(GenerateContainedEnemies());
            return monsterDen;
        }

        public override int PassesBy(IntVector2 direction, int amount)
        {
            var difference = 0;

            if (direction == Directions.Up)
            {
                difference = (_centerpoint.Y + _radius) - amount;
            }
            else if (direction == Directions.Right)
            {
                difference = (_centerpoint.X + _radius) - amount;
            }
            else if (direction == Directions.Down)
            {
                difference = amount - _centerpoint.Y;
            }
            else if (direction == Directions.Left)
            {
                difference = amount - (_centerpoint.X - _radius);
            }
            else throw new System.ArgumentException($" Expected a cardinal direction.  Cannot operate on {direction}.");

            return Mathf.Max(0, difference);
        }

        public override bool Contains(IntVector2 position)
        {
            if (position.X < _centerpoint.X - _radius ||
                position.X > _centerpoint.X + _radius ||
                position.Y > _centerpoint.Y + _radius ||
                position.Y < _centerpoint.Y)
            {
                return false;
            }
            else
            {
                var maxHeightAtDistance = _centerpoint.Y + _radius - DistanceFromCenterpoint(position.X);
                return position.Y <= maxHeightAtDistance;
            }
        }

        public override IntVector2 GetRandomPoint()
        {
            var randomX = Chance.Range(_centerpoint.X - _radius, _centerpoint.X + _radius + 1);
            var randomY = Chance.Range(_centerpoint.Y, _centerpoint.Y + (_radius - DistanceFromCenterpoint(randomX)));
            return new IntVector2(randomX, randomY);
        }

        public override int GetMaximalValue(IntVector2 direction)
        {
            if (direction == Directions.Up) return _centerpoint.Y + _radius;
            else if (direction == Directions.Right) return _centerpoint.X + _radius;
            else if (direction == Directions.Down) return _centerpoint.Y;
            else if (direction == Directions.Left) return _centerpoint.X - _radius;
            else throw new System.ArgumentException($" Expected a cardinal direction.  Cannot operate on {direction}.");
        }

        public override SpaceBuilder Align(IntVector2 direction, int amount)
        {
            if (direction == Directions.Up)
            {
                _centerpoint.Y = amount - _radius;
            }
            else if (direction == Directions.Right)
            {
                _centerpoint.X = amount - _radius;
            }
            else if (direction == Directions.Down)
            {
                _centerpoint.Y = amount;
            }
            else if (direction == Directions.Left)
            {
                _centerpoint.X = amount + _radius;
            }
            else throw new System.ArgumentException($" Expected a cardinal direction.  Cannot operate on {direction}.");

            OnSpaceBuilderChanged.Raise(this);

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

            SetRadius(_radius - difference);
            Clamp(direction, amount);
        }

        private List<EnemySpawn> GenerateContainedEnemies()
        {
            var containedEnemies = new List<EnemySpawn>();

            if (_allowEnemies)
            {
                var riskPoints = EnemyPicker.DetermineRiskPoints(_chunkBuilder.Depth, _chunkBuilder.Remoteness);
                riskPoints = Mathf.Max(riskPoints, 10);
                riskPoints += _extraRiskPoints;

                var enemies = EnemyPicker.RequestEnemies(riskPoints, new EnemyRequestCriteria()
                {
                    HeightsAllowed = new Range(0, _radius / 2),
                    LengthsAllowed = new Range(0, _radius / 2)
                });

                foreach (var enemy in enemies)
                {
                    var requirementsForEnemy = EnemyPicker.GetRequirementsForEnemy(enemy);

                    var requiredOffset = requirementsForEnemy.Height;

                    var xPosition = Chance.Range(-_radius + requiredOffset, _radius - requiredOffset) + _centerpoint.X;

                    var position = new IntVector2(xPosition, _centerpoint.Y);

                    containedEnemies.Add(new EnemySpawn(position, enemy));
                }
            }

            return containedEnemies;
        }

        private int DistanceFromCenterpoint(int x) => Mathf.Abs(_centerpoint.X - x);
    }
}