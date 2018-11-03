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
        private readonly IntVector2 _centerpoint;
        private readonly int _radius;

        private static int _chunkSize => GameManager.Instance.Settings.ChunkSize;

        public MonsterDenBuilder(ChunkBuilder containingChunk)
            : base(containingChunk)
        {
            _centerpoint = new IntVector2(Random.Range(_containingChunk.BottomLeft.X, _containingChunk.TopRight.X),
                                          Random.Range(_containingChunk.BottomLeft.Y, _containingChunk.TopRight.Y));

            _radius = Random.Range(8, 20);
        }

        public override Spaces.Space Build()
        {
            var monsterDen = new MonsterDen(_centerpoint, _radius);
            monsterDen.AddEnemySpawns(GenerateContainedEnemies());
            return monsterDen;
        }

        public override SpaceBuilder Clamp(IntVector2 direction, int amount)
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

            return this;
        }

        private Dictionary<IntVector2, EnemyTypes> GenerateContainedEnemies()
        {
            var containedEnemies = new Dictionary<IntVector2, EnemyTypes>();

            var riskPoints = EnemyPicker.DetermineRiskPoints(_containingChunk.Depth, _containingChunk.Remoteness);
            riskPoints = Math.Max(riskPoints, 10);

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

                containedEnemies[position] = enemy;
            }

            return containedEnemies;
        }
    }
}