using UnityEngine;

namespace WorldObjects.Hazards
{
    public class SpikeHazard : Hazard, IDamaging, IImpulsive
    {
        public override IntVector2 AnchoringPosition => new IntVector2(Position - transform.up);

        [SerializeField]
        private int _damage = 25;
        public int Damage => _damage;

        public Vector2 GetImpulse(Vector2 velocity) => -velocity * .75f;

        protected override void InitializeEffects() => Effects = new HazardEffects[] { HazardEffects.Damage, HazardEffects.Impulse };

        public override void SetAnchor(Block anchor)
        {
            if (anchor != null)
            {
                anchor.OnBlockCrumbled += OnAnchorRemoved;
                anchor.OnBlockDestroyed += OnAnchorRemoved;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnAnchorRemoved(Block anchor)
        {
            anchor.OnBlockCrumbled -= OnAnchorRemoved;
            anchor.OnBlockDestroyed -= OnAnchorRemoved;

            OnHazardDestroyed.Raise(this);
            Destroy(gameObject);
        }

        public override string ObjectName => $"Spike {Position}";

        protected override void OnDestroyed() { }
    }
}