using UnityEngine;

namespace WorldObjects
{
    [RequireComponent(typeof(Collider2D))]
    public abstract class Hazard : WorldObject
    {
        public SmartEvent<Hazard> OnHazardDestroyed = new SmartEvent<Hazard>();

        public abstract IntVector2 AnchoringPosition { get; }
        public HazardEffects[] Effects { get; protected set; }

        private void Awake() => InitializeEffects();

        protected abstract void InitializeEffects();

        public abstract void SetAnchor(Block anchorBlock);
    }
}