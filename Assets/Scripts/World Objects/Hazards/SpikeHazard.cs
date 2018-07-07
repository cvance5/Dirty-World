﻿using UnityEngine;

namespace WorldObjects.Hazards
{
    public class SpikeHazard : Hazard, IDamaging, IImpulsive
    {
        public override IntVector2 AnchoringPosition => new IntVector2(Position - transform.up);

        public int GetDamage() => 25;
        public Vector2 GetImpulse(Vector2 velocity) => -velocity * .75f;

        protected override void InitializeEffects() => Effects = new HazardEffects[] { HazardEffects.Damage, HazardEffects.Impulse };

        public override void SetAnchor(Block anchor)
        {
            if (anchor != null)
            {
                anchor.OnCrumbled += OnAnchorRemoved;
                anchor.OnDestroyed += OnAnchorRemoved;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnAnchorRemoved(Block anchor)
        {
            anchor.OnCrumbled -= OnAnchorRemoved;
            anchor.OnDestroyed -= OnAnchorRemoved;

            OnHazardDestroyed.Raise(this);
            Destroy(gameObject);
        }
    }
}