using UnityEngine;

namespace WorldObjects
{
    public abstract class WorldObject : MonoBehaviour, ITrackable
    {
        public SmartEvent<WorldObject> OnWorldObjectDestroyed = new SmartEvent<WorldObject>();

        public IntVector2 Position => new IntVector2(transform.position);

        public abstract string ObjectName { get; }
        public override string ToString() => ObjectName;

        private static bool _isQuitting = false;

        protected void OnDestroy()
        {
            if(!_isQuitting)
            {
                OnWorldObjectDestroyed.Raise(this);
                OnDestroyed();
            }
        }

        protected abstract void OnDestroyed();

        private void OnApplicationQuit()
        {
            _isQuitting = true;
        }
    }
}