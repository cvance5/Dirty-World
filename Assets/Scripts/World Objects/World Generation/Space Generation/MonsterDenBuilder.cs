using System;
using System.Collections.Generic;
using UnityEngine;
using WorldObjects.Spaces;
using WorldObjects.WorldGeneration.EnemyGeneration;
using Random = UnityEngine.Random;

namespace WorldObjects.WorldGeneration.SpaceGeneration
{
    public class MonsterDenBuilder : SpaceBuilder
    {
        private IntVector2 _centerpoint;
        private int _radius;

        private int _extraRiskPoints;
        private bool _allowEnemies = true;

        private static int _chunkSize => GameManager.Instance.Settings.ChunkSize;

        public MonsterDenBuilder(ChunkBuilder containingChunk)
            : base(containingChunk)
        {
            _centerpoint = new IntVector2(Random.Range(_containingChunk.BottomLeft.X, _containingChunk.TopRight.X),
                                          Random.Range(_containingChunk.BottomLeft.Y, _containingChunk.TopRight.Y));

            _radius = Random.Range(8, 20);
        }

        public MonsterDenBuilder SetCenterpoint(IntVector2 centerpoint)
        {
            _centerpoint = centerpoint;
            return this;
        }

        public MonsterDenBuilder SetRadius(int radius)
        {
            _radius = radius;
            _radius = Mathf.Max(0, _radius);
            if (_radius == 0) SetAllowEnemies(false);
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

        public override Spaces.Space Build()
        {
            var monsterDen = new MonsterDen(_centerpoint, _radius);
            monsterDen.AddEnemySpawns(GenerateContainedEnemies());
            return ApplyModifiers(monsterDen);
        }

        protected override void Clamp(IntVector2 direction, int amount)
        {
            if (direction == Directions.Up)
            {
                var overlap = (_centerpoint.Y + _radius) - amount;
                if (overlap > 0)
                {
                    _centerpoint.Y -= overlap;
                }
            }
            else if (direction == Directions.Right)
            {
                var overlap = (_centerpoint.X + _radius) - amount;
                if (overlap > 0)
                {
                    _centerpoint.X -= overlap;
                }
            }
            else if (direction == Directions.Down)
            {
                _centerpoint.Y = Mathf.Max(_centerpoint.Y, amount);
            }
            else if (direction == Directions.Left)
            {
                var overlap = (_centerpoint.X - _radius) - amount;

                if (overlap < 0)
                {
                    _centerpoint.X -= overlap;
                }
            }
            else throw new ArgumentException($" Expected a cardinal direction.  Cannot operate on {direction}.");
        }

        protected override void Cut(IntVector2 direction, int amount)
        {
            int difference;

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
                difference = _centerpoint.Y - amount;
            }
            else if (direction == Directions.Left)
            {
                difference = -((_centerpoint.X - _radius) - amount);
            }
            else throw new ArgumentException($" Expected a cardinal direction.  Cannot operate on {direction}.");

            SetRadius(_radius - difference);
            Clamp(direction, amount);
        }

        private List<EnemySpawn> GenerateContainedEnemies()
        {
            var containedEnemies = new List<EnemySpawn>();

            if (_allowEnemies)
            {
                var riskPoints = EnemyPicker.DetermineRiskPoints(_containingChunk.Depth, _containingChunk.Remoteness);
                riskPoints = Math.Max(riskPoints, 10);
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

                    var xPosition = Random.Range(-_radius + requiredOffset, _radius - requiredOffset) + _centerpoint.X;

                    var position = new IntVector2(xPosition, _centerpoint.Y);

                    containedEnemies.Add(new EnemySpawn(position, enemy));
                }
            }

            return containedEnemies;
        }
    }
}