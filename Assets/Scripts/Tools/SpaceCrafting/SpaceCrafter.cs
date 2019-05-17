using MathConcepts;
using System.Collections.Generic;
using UnityEngine;
using WorldObjects.Blocks;
using WorldObjects.Spaces;
using WorldObjects.WorldGeneration.EnemyGeneration;
using Space = WorldObjects.Spaces.Space;

namespace Tools.SpaceCrafting
{
    public abstract class SpaceCrafter : MonoBehaviour
    {
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

        public void InitializeFromSpace(Space space)
        {
            InitializeFromSpaceRaw(space);

            InitializeEnemySpawns(space.EnemySpawns);
            InitializeBlockOverrides(space.BlockOverrides);
        }
        protected abstract void InitializeFromSpaceRaw(Space space);

        private void InitializeEnemySpawns(List<EnemySpawn> spawns)
        {
            foreach (var spawn in spawns)
            {
                var crafter = AddEnemySpawnCrafter();
                crafter.Type = spawn.Type;
                crafter.transform.position = spawn.Position;
            }
        }

        private void InitializeBlockOverrides(Dictionary<IntVector2, BlockTypes> blockOverrides)
        {
            foreach (var blockOverride in blockOverrides)
            {
                var crafter = AddBlockOverride();
                crafter.Type = blockOverride.Value;
                crafter.transform.position = blockOverride.Key;
            }
        }

        public Space Build()
        {
            var space = RawBuild();
            space.AddEnemySpawns(BuildEnemySpawns());
            space.AddBlockOverrides(BuildBlockOverrides());
            return space;
        }

        protected abstract Space RawBuild();

        public List<IntVector2> GetAffectedChunks()
        {
            var chunksAffected = new List<IntVector2>();

            var minXChunk = MinX / SpaceCraftingManager.CHUNK_SIZE;
            var maxXChunk = MaxX / SpaceCraftingManager.CHUNK_SIZE;

            var minYChunk = MinY / SpaceCraftingManager.CHUNK_SIZE;
            var maxYChunk = MaxY / SpaceCraftingManager.CHUNK_SIZE;

            for (var x = minXChunk; x <= maxXChunk; x++)
            {
                for (var y = minYChunk; y <= maxYChunk; y++)
                {
                    chunksAffected.Add(new IntVector2(x, y));
                }
            }

            return chunksAffected;
        }

        public BlockOverrideCrafter AddBlockOverride()
        {
            var crafterObject = new GameObject("Block Override");
            var blockOverrideCrafter = crafterObject.AddComponent<BlockOverrideCrafter>();
            blockOverrideCrafter.transform.SetParent(transform);

            return blockOverrideCrafter;
        }

        public EnemySpawnCrafter AddEnemySpawnCrafter()
        {
            var crafterObject = new GameObject("Enemy Spawn");
            var enemySpawnCrafter = crafterObject.AddComponent<EnemySpawnCrafter>();
            enemySpawnCrafter.transform.SetParent(transform);

            return enemySpawnCrafter;
        }

        private Dictionary<IntVector2, BlockTypes> BuildBlockOverrides()
        {
            var blockOverrides = new Dictionary<IntVector2, BlockTypes>();
            foreach (var blockOverride in transform.GetComponentInImmediateChildren<BlockOverrideCrafter>())
            {
                blockOverrides[blockOverride.Position] = blockOverride.Type;
            }

            return blockOverrides;
        }

        private List<EnemySpawn> BuildEnemySpawns()
        {
            var enemySpawns = new List<EnemySpawn>();
            foreach (var enemySpawn in transform.GetComponentInImmediateChildren<EnemySpawnCrafter>())
            {
                if (enemySpawn.Type != EnemyTypes.None)
                {
                    enemySpawns.Add(enemySpawn.Build());
                }
            }
            return enemySpawns;
        }
    }
}