namespace WorldObjects
{
    public abstract class Space : IBoundary
    {
        public string Name { get; protected set; }

        public abstract bool Contains(IntVector2 position);
        public abstract Block GetBlock(IntVector2 position);

        public override string ToString() => Name;
    }
}