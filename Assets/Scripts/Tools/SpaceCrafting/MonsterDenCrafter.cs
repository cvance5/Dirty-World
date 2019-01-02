using WorldObjects.Spaces;

namespace Tools.SpaceCrafting
{
    public class MonsterDenCrafter : SpaceCrafter
    {
        public IntVector2 Centerpoint;
        public int Radius;

        public override bool IsValid => Radius >= 0;

        public override int MinX => Centerpoint.X - Radius;
        public override int MaxX => Centerpoint.X + Radius;

        public override int MinY => Centerpoint.Y;
        public override int MaxY => Centerpoint.Y + Radius;

        protected override void OnCrafterAwake()
        {
            gameObject.name = "Monster Den";

            Centerpoint = new IntVector2(0, 0);
            Radius = 3;
        }

        protected override Space RawBuild() => new MonsterDen(Centerpoint, Radius);
    }
}