using System.Collections;

namespace WorldObjects.Blocks
{
    public class StoneBlock : Block
    {
        public override string ObjectName => $"Stone {Position}";
        public override BlockTypes Type => BlockTypes.Stone;

        protected override IEnumerator Crumble()
        {
            yield return null;

            var neighbors = GameManager.World.GetNeighbors(this);

            foreach (var neighbor in neighbors)
            {
                neighbor.Hit(0, 100);
            }

            yield return base.Crumble();
        }

        protected override IEnumerator Destroy()
        {
            yield return null;

            var neighbors = GameManager.World.GetNeighbors(this);

            foreach (var neighbor in neighbors)
            {
                neighbor.Hit(0, 200);
            }

            yield return base.Destroy();
        }
    }
}