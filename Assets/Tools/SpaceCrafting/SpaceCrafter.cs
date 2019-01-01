using System.Collections.Generic;
using UnityEngine;

using Space = WorldObjects.Spaces.Space;

namespace Tools.SpaceCrafting
{
    public abstract class SpaceCrafter : MonoBehaviour
    {
        public static SmartEvent<SpaceCrafter> OnCrafterDestroyed = new SmartEvent<SpaceCrafter>();

        public Space Result { get; protected set; }

        public List<IntVector2> ChunksAffected { get; protected set; }
         = new List<IntVector2>();

        protected void Awake()
        {
            DontDestroyOnLoad(gameObject);
            OnCrafterAwake();
            BuildSpace();
        }

        private void OnValidate() => BuildSpace();

        protected abstract void OnCrafterAwake();

        protected abstract void BuildSpace();
        protected abstract void UpdateAffectedChunks();

        private void OnDestroy() => OnCrafterDestroyed.Raise(this);
    }
}