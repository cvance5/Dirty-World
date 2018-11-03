﻿using UnityEngine;
using WorldObjects.Blocks;

namespace WorldObjects.Hazards
{
    public class SpikeHazard : Hazard, IDamaging, IImpulsive
    {
        public override IntVector2 AnchoringPosition => new IntVector2(Position - transform.up);

#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly since we want it serialized by unity
        [SerializeField]
        private int _damage = 25;
        public int Damage => _damage;
#pragma warning restore IDE0044 // Add readonly modifier

        public Vector2 GetImpulse(Vector2 position, Vector2 velocity) => -velocity * .75f;

        protected override void InitializeEffects() => Effects = new HazardEffects[] { HazardEffects.Damage, HazardEffects.Impulse };

        public override bool SetAnchor(Block anchor)
        {
            if (anchor != null)
            {
                anchor.OnBlockCrumbled += OnAnchorRemoved;
                anchor.OnBlockDestroyed += OnAnchorRemoved;
                return true;
            }
            else
            {
                Destroy(gameObject);
                return false;
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

        protected override void OnWorldObjectDestroy() { }
    }
}