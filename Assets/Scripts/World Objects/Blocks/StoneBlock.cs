namespace WorldObjects.Blocks
{
    public class StoneBlock : Block
    {
        public override string GetObjectName() => $"Stone {GetPosition()}";

        protected override void Crumble()
        {
            var neighbors = World.GetNeighbors(this);

            foreach (var neighbor in neighbors)
            {
                neighbor.Hit(0, 100);
            }

            base.Crumble();
        }

        protected override void Destroy()
        {
            var neighbors = World.GetNeighbors(this);

            foreach (var neighbor in neighbors)
            {
                neighbor.Hit(0, 200);
            }

            base.Destroy();
        }
    }
}