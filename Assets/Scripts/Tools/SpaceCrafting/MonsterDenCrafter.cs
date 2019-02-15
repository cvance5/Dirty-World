using WorldObjects.Spaces;

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

        public override void InitializeFromSpace(Space space)
        {
            var monsterDen = space as MonsterDen;
            transform.position = monsterDen.Centerpoint;
            Radius = monsterDen.Radius;

            InitializeEnemySpawns(monsterDen.EnemySpawns);
        }

        protected override Space RawBuild() => new MonsterDen(Centerpoint, Radius);
    }
}