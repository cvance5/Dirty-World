using MathConcepts;
using MathConcepts.Geometry;
using System.Collections.Generic;
using WorldObjects.Spaces;
using WorldObjects.WorldGeneration.SpaceGeneration;

namespace Tools.SpaceCrafting
{
    public class MonsterDenCrafter : SpaceCrafter
    {
        public int Radius;
        public IntVector2 Centerpoint => new IntVector2(transform.position);

        public override bool IsValid => Radius >= 0;

        public override int MinX => Centerpoint.X - Radius;
        public override int MaxX => Centerpoint.X + Radius;

        public override int MinY => Centerpoint.Y;
        public override int MaxY => Centerpoint.Y + Radius;

        protected override void OnCrafterAwake()
        {
            gameObject.name = "Monster Den";

            Radius = 3;
        }

        protected override void InitializeFromSpaceRaw(Space space)
        {
            var monsterDen = space;
            transform.position = new UnityEngine.Vector3((space.Extents.Min.X + space.Extents.Max.X) / 2, space.Extents.Max.Y);
            Radius = space.Extents.Max.Y - space.Extents.Min.Y;
        }

        protected override Space RawBuild()
        {
            var extents = new Extents(new List<IntVector2>()
            {
                new IntVector2(Centerpoint.X - Radius, Centerpoint.Y),
                new IntVector2(Centerpoint.X, Centerpoint.Y + Radius),
                new IntVector2(Centerpoint.X + Radius, Centerpoint.Y)
            });

            return new Space($"Monster Den {SpaceNamer.GetName()}", extents);
        }
    }
}