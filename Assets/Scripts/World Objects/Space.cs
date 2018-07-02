using UnityEngine;

namespace WorldObjects
{
    public abstract class Space
    {
        public abstract bool Contains(IntVector2 position);
        public abstract Block GetBlock(IntVector2 position);
    }
}