using System.Collections.Generic;
using UnityEngine;

using Space = WorldObjects.Spaces.Space;

namespace Tools.SpaceCrafting
{
    public abstract class SpaceCrafter : MonoBehaviour
    {
        public static SmartEvent<SpaceCrafter> OnCrafterDestroyed = new SmartEvent<SpaceCrafter>();

        public abstract bool IsValid { get; }

        public abstract int MinX { get; }
        public abstract int MaxX { get; }
        public abstract int MinY { get; }
        public abstract int MaxY { get; }

        protected void Awake()
        {
            DontDestroyOnLoad(gameObject);
            OnCrafterAwake();
        }

        protected abstract void OnCrafterAwake();

        public abstract Space Build();

        public List<IntVector2> GetAffectedChunks()
        {
            var chunksAffected = new List<IntVector2>();

            var minXChunk = MinX / SpaceCraftingManager.ChunkSize;
            var maxXChunk = MaxX / SpaceCraftingManager.ChunkSize;

            var minYChunk = MinY / SpaceCraftingManager.ChunkSize;
            var maxYChunk = MaxY / SpaceCraftingManager.ChunkSize;

            for (var x = minXChunk; x <= maxXChunk; x++)
            {
                for (var y = minYChunk; y <= maxYChunk; y++)
                {
                    chunksAffected.Add(new IntVector2(x, y));
                }
            }

            return chunksAffected;
        }

        private void OnDestroy() => OnCrafterDestroyed.Raise(this);
    }
}