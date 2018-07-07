using System.Collections;

namespace WorldObjects.Blocks
{
    public class StoneBlock : Block
    {
        public override string GetObjectName() => $"Stone {GetPosition()}";

        protected override IEnumerator Crumble()
        {
            yield return null;

            var neighbors = World.GetNeighbors(this);

            foreach (var neighbor in neighbors)
            {
                neighbor.Hit(0, 100);
            }

            yield return base.Crumble();
        }

        protected override IEnumerator Destroy()
        {
            yield return null;

            var neighbors = World.GetNeighbors(this);

            foreach (var neighbor in neighbors)
            {
                neighbor.Hit(0, 200);
            }

            yield return base.Destroy();
        }
    }
}