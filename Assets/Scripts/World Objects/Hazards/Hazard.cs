﻿namespace WorldObjects.Hazards
{
    public abstract class Hazard : WorldObject
    {
        public SmartEvent<Hazard> OnHazardChanged = new SmartEvent<Hazard>();
        public SmartEvent<Hazard> OnHazardDestroyed = new SmartEvent<Hazard>();

        public abstract HazardEffects[] Effects { get; }
        public abstract HazardTypes Type { get; }
    }
}