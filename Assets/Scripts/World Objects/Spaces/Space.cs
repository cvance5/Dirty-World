using System.Collections.Generic;
using WorldObjects.Blocks;
using WorldObjects.Spaces.Geometry;
using WorldObjects.WorldGeneration.FeatureGeneration;
using WorldObjects.WorldGeneration.HazardGeneration;
using WorldObjects.WorldGeneration.PropGeneration;

namespace WorldObjects.Spaces
{
    public abstract class Space
    {
        public abstract string Name { get; }
        public Extents Extents { get; } = new Extents();

        protected readonly List<EnemySpawn> _enemySpawns = new List<EnemySpawn>();
        public List<EnemySpawn> EnemySpawns => new List<EnemySpawn>(_enemySpawns);
        public void AddEnemySpawns(List<EnemySpawn> enemySpawns) => _enemySpawns.AddRange(enemySpawns);

        protected readonly List<HazardBuilder> _hazardBuilders = new List<HazardBuilder>();
        public List<HazardBuilder> HazardBuilders => new List<HazardBuilder>(_hazardBuilders);
        public void AddHazardBuilders(List<HazardBuilder> hazardBuilders) => _hazardBuilders.AddRange(hazardBuilders);

        protected readonly List<FeatureBuilder> _featureBuilders = new List<FeatureBuilder>();
        public List<FeatureBuilder> FeatureBuilders => new List<FeatureBuilder>(_featureBuilders);
        public void AddFeatureBuilder(FeatureBuilder featureBuilder) => _featureBuilders.Add(featureBuilder);
        public void AddFeatureBuilders(List<FeatureBuilder> featureBuilders) => _featureBuilders.AddRange(featureBuilders);

        protected readonly List<ModifierTypes> _modifiers = new List<ModifierTypes>();
        public List<ModifierTypes> Modifiers => new List<ModifierTypes>(_modifiers);
        public void AddModifier(ModifierTypes modifier) => _modifiers.Add(modifier);
        public void AddModifiers(List<ModifierTypes> modifiers) => _modifiers.AddRange(modifiers);

        protected readonly Dictionary<IntVector2, PropTypes> _props = new Dictionary<IntVector2, PropTypes>();
        public virtual PropTypes GetProp(IntVector2 position) => _props.TryGetValue(position, out var type) ? type : PropTypes.None;
        public void AddProp(IntVector2 position, PropTypes type) => _props[position] = type;

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

        public abstract IntVector2 GetRandomPosition();

        public List<EnemySpawn> GetEnemySpawnsInChunk(Chunk chunk)
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

        public List<HazardBuilder> GetHazardBuildersInChunk(Chunk chunk)
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

        public List<FeatureBuilder> GetFeatureBuildersInChunk(Chunk chunk)
        {
            var featureBuildersInChunk = new List<FeatureBuilder>();

            foreach (var featureBuilder in _featureBuilders)
            {
                if (chunk.Contains(featureBuilder.Position))
                {
                    featureBuildersInChunk.Add(featureBuilder);
                }
            }

            foreach (var featureBuilder in featureBuildersInChunk)
            {
                _featureBuilders.Remove(featureBuilder);
            }

            return featureBuildersInChunk;
        }

        public int GetMaximalValue(IntVector2 direction)
        {
            if (direction == Directions.Up) return Extents.Max.Y;
            else if (direction == Directions.Right) return Extents.Max.X;
            else if (direction == Directions.Down) return Extents.Min.Y;
            else if (direction == Directions.Left) return Extents.Min.X;
            else throw new System.ArgumentException($"Expected a cardinal direction.  Cannot operate on {direction}.");
        }

        public override string ToString() => Name;

        public bool Equals(Space other) => Extents.Equals(other.Extents);
        public override bool Equals(object obj) => obj is Space ? Equals(obj as Space) : false;

        public override int GetHashCode()
        {
            var hashCode = -1094564240;
            hashCode = hashCode * -1521134295 + EqualityComparer<Extents>.Default.GetHashCode(Extents);
            return hashCode;
        }
    }
}