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

        protected virtual void Awake()
        {
            SceneHelper.OnSceneIsEnding += OnApplicationQuit;
            OnWorldObjectAwake();
        }

        protected virtual void OnWorldObjectAwake() { }

        protected void OnDestroy()
        {
            if (!_isQuitting)
            {
                OnWorldObjectDestroyed.Raise(this);
                OnWorldObjectDestroy();
            }
        }

        protected abstract void OnWorldObjectDestroy();

        private void OnApplicationQuit()
        {
            SceneHelper.OnSceneIsEnding -= OnApplicationQuit;
            _isQuitting = true;
        }
    }
}