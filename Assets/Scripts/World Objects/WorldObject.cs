using UnityEngine;

namespace WorldObjects
{
    public abstract class WorldObject : MonoBehaviour, ITrackable
    {
        public abstract string GetObjectName();
        public IntVector2 GetPosition() => new IntVector2(transform.position);

        public override string ToString() => GetObjectName();
    }
}