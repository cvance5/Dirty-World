using UnityEngine;
using WorldObjects.Blocks;
using WorldObjects.Hazards;

namespace WorldObjects.Spaces
{
    public class MonsterDen : Space
    {
        public override string Name => $"Monster Den starting {Centerpoint} extending {Radius}.";

        public override bool IsHazardous => true;

        public IntVector2 Centerpoint { get; }
        public int Radius { get; }

        public MonsterDen(IntVector2 centerpoint, int radius)
        {
            Centerpoint = centerpoint;
            Radius = radius;

            Extents.Add(new IntVector2(Centerpoint.X - Radius, Centerpoint.Y));
            Extents.Add(new IntVector2(Centerpoint.X + Radius, Centerpoint.Y));
            Extents.Add(new IntVector2(Centerpoint.X, Centerpoint.Y + Radius));
        }

        public override bool Contains(IntVector2 position)
        {
            if (position.X < Centerpoint.X - Radius ||
                position.X > Centerpoint.X + Radius ||
                position.Y > Centerpoint.Y + Radius ||
                position.Y < Centerpoint.Y)
            {
                return false;
            }
            else
            {
                var distanceFromCenterpoint = Mathf.Abs(Centerpoint.X - position.X);
                var maxHeightAtDistance = Centerpoint.Y + Radius - distanceFromCenterpoint;

                return position.Y <= maxHeightAtDistance;
            }
        }

        public override BlockTypes GetBlock(IntVector2 position) => BlockTypes.None;
        public override HazardTypes GetHazard(IntVector2 position) => HazardTypes.None;
    }
}