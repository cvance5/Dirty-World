namespace WorldObjects.WorldGeneration
{
    public abstract class SpaceBuilder
    {
        public abstract Space Build();
        public abstract SpaceBuilder Clamp(IntVector2 direction, int amount);
    }
}