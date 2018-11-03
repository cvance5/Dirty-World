using UnityEngine;
using WorldObjects.Blocks;

namespace WorldObjects.Hazards
{
    public abstract class Hazard : WorldObject
    {
        public SmartEvent<Hazard> OnHazardDestroyed = new SmartEvent<Hazard>();

        public abstract IntVector2 AnchoringPosition { get; }
        public HazardEffects[] Effects { get; protected set; }

        protected override void OnWorldObjectAwake() => InitializeEffects();

        protected abstract void InitializeEffects();

        public abstract bool SetAnchor(Block anchorBlock);
    }
}