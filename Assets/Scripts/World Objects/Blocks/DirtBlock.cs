using UnityEngine;

namespace WorldObjects.Blocks
{
    public class DirtBlock : Block
    {
        public override void HandleNeighborUpdate()
        {
            var neighbors = World.GetNeighbors(this);

            if (neighbors.Count == 0)
            {
                Crumble();
            }
        }

        protected override void Stabilize()
        {
            base.Stabilize();
            _stability = Mathf.Min(_health, 100);
        }
    }
}
