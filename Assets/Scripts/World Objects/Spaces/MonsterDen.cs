using System.Collections.Generic;
using UnityEngine;
using WorldObjects.Blocks;

namespace WorldObjects.Spaces
{
    public class MonsterDen : Space
    {
        public override string Name => $"Monster Den starting {Centerpoint} extending {Radius}.";

        public IntVector2 Centerpoint { get; }
        public int Radius { get; }

        public MonsterDen(IntVector2 centerpoint, int radius)
        {
            Centerpoint = centerpoint;
            Radius = radius;

            Extents.AddShape(new List<IntVector2>()
            {
                new IntVector2(Centerpoint.X - Radius, Centerpoint.Y),
                new IntVector2(Centerpoint.X + Radius, Centerpoint.Y),
                new IntVector2(Centerpoint.X, Centerpoint.Y + Radius)
            });
        }

        public override IntVector2 GetRandomPosition()
        {
            var randomX = Chance.Range(Centerpoint.X - Radius, Centerpoint.X + Radius);
            var randomY = Chance.Range(Centerpoint.Y, Centerpoint.Y + Radius - DistanceFromCenterpoint(randomX));

            return new IntVector2(randomX, randomY);
        }

        public override BlockTypes GetBlockType(IntVector2 position)
        {
            if (!Extents.Contains(position)) throw new System.ArgumentOutOfRangeException($"{Name} does not contain {position}.  Cannot get block.");
            else return _blockOverride.TryGetValue(position, out var overrideType) ? overrideType : BlockTypes.None;
        }

        private int DistanceFromCenterpoint(int x) => Mathf.Abs(Centerpoint.X - x);
    }
}