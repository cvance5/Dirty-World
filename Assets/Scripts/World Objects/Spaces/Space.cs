﻿using System.Collections.Generic;
using System.Linq;
using WorldObjects.Blocks;
using WorldObjects.WorldGeneration.FeatureGeneration;
using WorldObjects.WorldGeneration.HazardGeneration;

namespace WorldObjects.Spaces
{
    public abstract class Space : IBoundary
    {
        public abstract string Name { get; }
        public List<IntVector2> Extents { get; protected set; } = new List<IntVector2>();

        protected readonly List<EnemySpawn> _enemySpawns = new List<EnemySpawn>();
        public virtual List<EnemySpawn> EnemySpawns => new List<EnemySpawn>(_enemySpawns);
        public void AddEnemySpawns(List<EnemySpawn> enemySpawns) => _enemySpawns.AddRange(enemySpawns);

        protected readonly List<HazardBuilder> _hazardBuilders = new List<HazardBuilder>();
        public virtual List<HazardBuilder> HazardBuilders => new List<HazardBuilder>(_hazardBuilders);
        public void AddHazardBuilders(List<HazardBuilder> hazardBuilders) => _hazardBuilders.AddRange(hazardBuilders);

        protected readonly List<ModifierTypes> _modifiers = new List<ModifierTypes>();
        public virtual List<ModifierTypes> Modifiers => new List<ModifierTypes>(_modifiers);
        internal void AddModifier(ModifierTypes modifier) => _modifiers.Add(modifier);
        public void AddModifiers(List<ModifierTypes> modifiers) => _modifiers.AddRange(modifiers);

        protected readonly Dictionary<IntVector2, FeatureTypes> _features = new Dictionary<IntVector2, FeatureTypes>();
        public virtual FeatureTypes GetFeatureType(IntVector2 position) => _features.TryGetValue(position, out var type) ? type : FeatureTypes.None;
        public void AddFeature(IntVector2 position, FeatureTypes type) => _features[position] = type;

        protected readonly Dictionary<IntVector2, BlockTypes> _blockOverride = new Dictionary<IntVector2, BlockTypes>();
        public Dictionary<IntVector2, BlockTypes> BlockOverrides => new Dictionary<IntVector2, BlockTypes>(_blockOverride);
        public abstract BlockTypes GetBlockType(IntVector2 position);
        public void AddBlockOverrides(Dictionary<IntVector2, BlockTypes> newOverrides)
        {
            foreach (var blockOverride in newOverrides)
            {
                _blockOverride[blockOverride.Key] = blockOverride.Value;
            }
        }

        public abstract bool Contains(IntVector2 position);
        public abstract IntVector2 GetRandomPosition();

        public virtual List<EnemySpawn> GetEnemySpawnsInChunk(Chunk chunk)
        {
            var enemySpawnsInChunk = new List<EnemySpawn>();

            foreach (var enemySpawn in _enemySpawns)
            {
                if (chunk.Contains(enemySpawn.Position))
                {
                    enemySpawnsInChunk.Add(enemySpawn);
                }
            }

            foreach (var enemySpawn in enemySpawnsInChunk)
            {
                _enemySpawns.Remove(enemySpawn);
            }

            return enemySpawnsInChunk;
        }

        public virtual List<HazardBuilder> GetHazardBuildersInChunk(Chunk chunk)
        {
            var hazardBuildersInChunk = new List<HazardBuilder>();

            foreach (var hazardBuilder in _hazardBuilders)
            {
                if (chunk.Contains(hazardBuilder.Position))
                {
                    hazardBuildersInChunk.Add(hazardBuilder);
                }
            }

            foreach (var hazardSpawn in hazardBuildersInChunk)
            {
                _hazardBuilders.Remove(hazardSpawn);
            }

            return hazardBuildersInChunk;
        }

        public int GetMaximalValue(IntVector2 direction)
        {
            if (direction == Directions.Up) return Extents.Max(extent => extent.Y);
            else if (direction == Directions.Right) return Extents.Max(extent => extent.X);
            else if (direction == Directions.Down) return Extents.Min(extent => extent.Y);
            else if (direction == Directions.Left) return Extents.Min(Extents => Extents.X);
            else throw new System.ArgumentException($"Expected a cardinal direction.  Cannot operate on {direction}.");
        }

        public override string ToString() => Name;

        public bool Equals(Space other)
        {
            var isEqual = true;

            if (Extents.Count != other.Extents.Count)
            {
                isEqual = false;
            }
            else
            {
                foreach (var extent in other.Extents)
                {
                    if (!Extents.Contains(extent))
                    {
                        isEqual = false;
                        break;
                    }
                }
            }

            return isEqual;
        }

        public override bool Equals(object obj) => obj is Space ? Equals(obj as Space) : false;

        public override int GetHashCode() => 623212078 + EqualityComparer<List<IntVector2>>.Default.GetHashCode(Extents);
    }
}