
using System.Collections.Generic;
using UnityEngine;
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
        public abstract void InitializeFromJSON(string json);
        public abstract void InitializeFromSpace(Space space);

        public void InitializeEnemySpawns(List<EnemySpawn> spawns)
        {
            foreach(var spawn in spawns)
            {
                var crafter = AddEnemySpawnCrafter();
                crafter.Type = spawn.Type;
                crafter.transform.position = spawn.Position;
            }
        }

        public Space Build()
        {
            var space = RawBuild();
            space.AddEnemySpawns(BuildEnemySpawns());
            return space;
        }

        protected abstract Space RawBuild();

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

        public EnemySpawnCrafter AddEnemySpawnCrafter()
        {
            var crafterObject = new GameObject("Enemy Spawn");
            var enemySpawnCrafter = crafterObject.AddComponent<EnemySpawnCrafter>();
            enemySpawnCrafter.transform.SetParent(transform);

            return enemySpawnCrafter;
        }

        private List<EnemySpawn> BuildEnemySpawns()
        {
            var enemySpawns = new List<EnemySpawn>();
            foreach (var enemySpawn in transform.GetComponentsInChildren<EnemySpawnCrafter>())
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