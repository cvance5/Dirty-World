using UnityEngine;

namespace WorldObjects.Blocks
{
    public class DirtBlock : Block
    {
        private int _numNeighbors;

        public override string GetObjectName() => $"Dirt {Position}";

        public override void HandleNeighborUpdate()
        {
            UpdateStability();
        }

        protected override void Crumble()
        {
            base.Crumble();
            DestabalizeNeighbors(25);
        }

        protected override void Destroy()
        {
            base.Destroy();
            DestabalizeNeighbors(50);
        }

        protected override void Stabilize()
        {
            base.Stabilize();
            UpdateStability();
        }

        private void DestabalizeNeighbors(int stabilityThreshold)
        {
            var neighbors = World.GetNeighbors(this);

            foreach (var neighbor in neighbors)
            {
                if (neighbor.Stability <= stabilityThreshold)
                {
                    neighbor.Hit(0, stabilityThreshold);
                    Log.Info($"{this} has applied {stabilityThreshold} force to neighbor {neighbor}.");
                }
            }
        }

        private void UpdateStability()
        {
            _numNeighbors = World.GetNeighbors(this).Count;
            Stability = Mathf.Min(Health, _numNeighbors * 25);

            if (_numNeighbors == 0) Crumble();
        }
    }
}
