using System.Collections.Generic;
using WorldObjects.Blocks;
using WorldObjects.WorldGeneration.HazardGeneration;

namespace WorldObjects.Spaces
{
    public abstract class Space : IBoundary
    {
        public abstract string Name { get; }
        public List<IntVector2> Extents { get; protected set; } = new List<IntVector2>();
            
        public abstract int Area { get; }

        protected readonly List<EnemySpawn> _enemySpawns = new List<EnemySpawn>();
        public virtual List<EnemySpawn> EnemySpawns => new List<EnemySpawn>(_enemySpawns);

        protected readonly List<HazardBuilder> _hazardBuilders = new List<HazardBuilder>();
        public virtual List<HazardBuilder> HazardBuilders => new List<HazardBuilder>(_hazardBuilders);

        protected readonly List<ModifierTypes> _modifiers = new List<ModifierTypes>();
        public virtual List<ModifierTypes> Modifiers => new List<ModifierTypes>(_modifiers);

        public abstract bool Contains(IntVector2 position);
        public abstract IntVector2 GetRandomPosition();

        public abstract BlockTypes GetBlockType(IntVector2 position);

        public virtual void AddEnemySpawns(List<EnemySpawn> enemySpawns) => _enemySpawns.AddRange(enemySpawns);
        public virtual void AddHazardBuilders(List<HazardBuilder> hazardBuilders) => _hazardBuilders.AddRange(hazardBuilders);

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