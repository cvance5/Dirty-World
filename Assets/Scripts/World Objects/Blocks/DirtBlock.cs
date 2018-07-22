using System.Collections;
using UnityEngine;

namespace WorldObjects.Blocks
{
    public class DirtBlock : Block
    {
        private int _numNeighbors;

        public override string ObjectName => $"Dirt {Position}";

        public override void HandleNeighborUpdate()
        {
            UpdateStability();
        }

        protected override IEnumerator Crumble()
        {
            yield return base.Crumble();
            DestabalizeNeighbors(25);
        }

        protected override IEnumerator Destroy()
        {
            yield return base.Destroy();

            if (IsStable)
            {
                DestabalizeNeighbors(50);

            }
        }

        protected override IEnumerator Stabilize()
        {
            yield return base.Stabilize();
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
                    _log.Info($"{this} has applied {stabilityThreshold} force to neighbor {neighbor}.");
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
