using System.Collections.Generic;

namespace WorldObjects
{
    public abstract class Space : IBoundary
    {
        public string Name { get; protected set; }
        public List<IntVector2> Extents { get; protected set; }
            = new List<IntVector2>();

        public abstract bool IsHazardous { get; }

        public abstract bool Contains(IntVector2 position);
        public abstract BlockTypes GetBlock(IntVector2 position);
        public abstract HazardTypes GetHazard(IntVector2 position);

        public override string ToString() => Name;
    }
}