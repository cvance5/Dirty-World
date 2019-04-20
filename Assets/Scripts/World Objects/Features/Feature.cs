namespace WorldObjects.Features
{
    public abstract class Feature : WorldObject
    {
        public SmartEvent<Feature> OnFeatureChanged = new SmartEvent<Feature>();
        public SmartEvent<Feature> OnFeatureDestroyed = new SmartEvent<Feature>();
    }
}