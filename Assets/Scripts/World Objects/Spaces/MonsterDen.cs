using MathConcepts;
using System.Collections.Generic;
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
                new IntVector2(Centerpoint.X, Centerpoint.Y + Radius),
                new IntVector2(Centerpoint.X + Radius, Centerpoint.Y)
            });
        }
    }
}