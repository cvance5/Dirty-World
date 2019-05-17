using MathConcepts;
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
        private static bool _isLoadingNewScene = false;

        protected virtual void Awake()
        {
            SceneHelper.OnSceneIsEnding += OnSceneIsEnding;
            OnWorldObjectAwake();
        }

        protected virtual void OnWorldObjectAwake() { }

        protected void OnDestroy()
        {
            if (!_isQuitting)
            {
                if (_isLoadingNewScene)
                {
                    OnWorldObjectUnloaded();
                }
                else
                {
                    OnWorldObjectDestroyed.Raise(this);
                    OnWorldObjectDestroy();
                }
            }
        }

        protected virtual void OnWorldObjectDestroy() { }
        protected virtual void OnWorldObjectUnloaded() { }

        private void OnSceneIsEnding()
        {
            _isLoadingNewScene = true;
            SceneHelper.OnSceneIsEnding -= OnSceneIsEnding;
        }

        private void OnApplicationQuit()
        {
            SceneHelper.OnSceneIsEnding -= OnSceneIsEnding;
            _isQuitting = true;
        }
    }
}