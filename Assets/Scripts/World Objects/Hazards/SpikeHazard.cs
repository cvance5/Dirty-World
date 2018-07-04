using UnityEngine;

namespace WorldObjects.Hazards
{
    public class SpikeHazard : Hazard, IDamaging, IImpulsive
    {
        protected override void InitializeEffects() => Effects = new HazardEffects[] { HazardEffects.Damage, HazardEffects.Impulse };

        public int GetDamage() => 25;

        public Vector2 GetImpulse(Vector2 velocity)
        {
            return -velocity * .75f;
        }
    }
}