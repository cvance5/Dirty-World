﻿using System.Collections.Generic;
using WorldObjects.Blocks;
using WorldObjects.Hazards;
using WorldObjects.WorldGeneration.EnemyGeneration;

namespace WorldObjects.Spaces
{
    public abstract class Space : IBoundary
    {
        public abstract string Name { get; }
        public List<IntVector2> Extents { get; protected set; }
            = new List<IntVector2>();

        private readonly Dictionary<IntVector2, EnemyTypes> _enemySpawns
            = new Dictionary<IntVector2, EnemyTypes>();
        public Dictionary<IntVector2, EnemyTypes> EnemySpawns =>
            new Dictionary<IntVector2, EnemyTypes>(_enemySpawns);

        private readonly List<ModifierTypes> _modifiers = new List<ModifierTypes>();
        public virtual List<ModifierTypes> Modifiers => new List<ModifierTypes>(_modifiers);

        public abstract bool IsHazardous { get; }

        public abstract bool Contains(IntVector2 position);
        public abstract BlockTypes GetBlock(IntVector2 position);
        public abstract HazardTypes GetHazard(IntVector2 position);

        public void AddEnemySpawns(Dictionary<IntVector2, EnemyTypes> enemiesToSpawn)
        {
            foreach (var kvp in enemiesToSpawn)
            {
                _enemySpawns.Add(kvp.Key, kvp.Value);
            }
        }

        public Dictionary<IntVector2, EnemyTypes> GetEnemySpawnsInChunk(Chunk chunk)
        {
            var enemySpawnsInChunk = new Dictionary<IntVector2, EnemyTypes>();

            foreach (var kvp in _enemySpawns)
            {
                if (chunk.Contains(kvp.Key))
                {
                    enemySpawnsInChunk.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (var kvp in enemySpawnsInChunk)
            {
                _enemySpawns.Remove(kvp.Key);
            }

            return enemySpawnsInChunk;
        }

        public void AddModifier(ModifierTypes modifier)
        {
            if (!_modifiers.Contains(modifier))
            {
                _modifiers.Add(modifier);
            }
            else throw new System.InvalidOperationException($"Cannot apply the same modifier twice.  Applied {modifier} twice to {Name}.");
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