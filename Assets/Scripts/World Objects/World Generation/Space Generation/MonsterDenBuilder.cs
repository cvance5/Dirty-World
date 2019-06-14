using MathConcepts;
using MathConcepts.Geometry;
using System.Collections.Generic;
using UnityEngine;
using WorldObjects.Spaces;
using WorldObjects.WorldGeneration.EnemyGeneration;

namespace WorldObjects.WorldGeneration.SpaceGeneration
{
    public class MonsterDenBuilder : SpaceBuilder
    {
        public override bool IsValid => Origin != null && _radius > 0;

        private int _radius;

        private int _extraRiskPoints;
        private bool _allowEnemies = true;

        public MonsterDenBuilder(ChunkBuilder chunkBuilder)
            : base(chunkBuilder)
        {
            Origin = new IntVector2(Chance.Range(_chunkBuilder.BottomLeftCorner.X, _chunkBuilder.TopRightCorner.X),
                                          Chance.Range(_chunkBuilder.BottomLeftCorner.Y, _chunkBuilder.TopRightCorner.Y));

            _radius = Chance.Range(8, 20);

            Recalculate();
        }

        public MonsterDenBuilder SetCenterpoint(IntVector2 centerpoint)
        {
            Origin = centerpoint;

            Recalculate();

            return this;
        }

        public MonsterDenBuilder SetRadius(int radius)
        {
            _radius = radius;
            _radius = Mathf.Max(0, _radius);
            if (_radius == 0) SetAllowEnemies(false);

            Recalculate();
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
            var extents = new Extents(new Shape(new List<IntVector2>()
            {
                new IntVector2(Origin.X - _radius, Origin.Y),
                new IntVector2(Origin.X, Origin.Y + _radius),
                new IntVector2(Origin.X + _radius, Origin.Y)
            }));

            return new Spaces.Space($"Monster Den {Name}", extents);
        }

        public override bool Contains(IntVector2 position)
        {
            if (position.X < Origin.X - _radius ||
                position.X > Origin.X + _radius ||
                position.Y > Origin.Y + _radius ||
                position.Y < Origin.Y)
            {
                return false;
            }
            else
            {
                var maxHeightAtDistance = Origin.Y + _radius - DistanceFromCenterpoint(position.X);
                return position.Y <= maxHeightAtDistance;
            }
        }

        public override void Cut(IntVector2 direction, int amount)
        {
            var difference = DistanceFrom(direction, amount);

            if (difference > 0)
            {
                SetRadius(_radius - difference);
                Clamp(direction, amount);
            }
        }

        protected override void Recalculate()
        {
            _maximalValues[Directions.Up] = Origin.Y + _radius;
            _maximalValues[Directions.Right] = Origin.X + _radius;
            _maximalValues[Directions.Down] = Origin.Y;
            _maximalValues[Directions.Left] = Origin.X - _radius;

            OnSpaceBuilderChanged.Raise(this);
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

                    var xPosition = Chance.Range(-_radius + requiredOffset, _radius - requiredOffset) + Origin.X;

                    var position = new IntVector2(xPosition, Origin.Y);

                    containedEnemies.Add(new EnemySpawn(position, enemy));
                }
            }

            return containedEnemies;
        }

        private int DistanceFromCenterpoint(int x) => Mathf.Abs(Origin.X - x);
    }
}