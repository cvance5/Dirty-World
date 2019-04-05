using Data;
using Items.ItemActors;
using System;
using System.Collections.Generic;
using UnityEngine;
using WorldObjects.Actors;
using WorldObjects.Actors.Enemies;
using WorldObjects.Blocks;
using WorldObjects.Features;
using WorldObjects.Hazards;

namespace WorldObjects
{
    public class Chunk : MonoBehaviour, ITrackable, IBoundary
    {
        public static SmartEvent<Chunk> OnChunkChanged = new SmartEvent<Chunk>();

        public SmartEvent<Chunk> OnChunkReady = new SmartEvent<Chunk>();
        public SmartEvent<Chunk> OnChunkActivated = new SmartEvent<Chunk>();

        public IntVector2 Position => new IntVector2(transform.position);
        public IntVector2 BottomLeftCorner { get; protected set; }
        public IntVector2 TopRightCorner { get; protected set; }

        public List<Hazard> Hazards { get; private set; } = new List<Hazard>();
        public List<EnemyHealth> Enemies { get; private set; } = new List<EnemyHealth>();
        public List<ItemActor> Items { get; private set; } = new List<ItemActor>();

        public Dictionary<IntVector2, Block> BlockMap { get; private set; } = new Dictionary<IntVector2, Block>();
        public Dictionary<IntVector2, Feature> FeatureMap { get; private set; } = new Dictionary<IntVector2, Feature>();

        public List<string> SpacesUsed { get; private set; } = new List<string>();

        public ChunkState State { get; private set; } = ChunkState.Constructing;

        public void AssignExtents(IntVector2 bottomLeftCorner, IntVector2 topRightCorner)
        {
            BottomLeftCorner = bottomLeftCorner;
            TopRightCorner = topRightCorner;
        }

        public bool Contains(IntVector2 position) =>
            position.X >= BottomLeftCorner.X &&
            position.Y >= BottomLeftCorner.Y &&
            position.X <= TopRightCorner.X &&
            position.Y <= TopRightCorner.Y;

        public void Register(Block block)
        {
            BlockMap[block.Position] = block;
            block.transform.SetParent(transform, true);

            block.OnBlockDestroyed += OnBlockDestroyed;
            block.OnBlockCrumbled += OnBlockCrumbled;
            block.OnBlockStabilized += OnBlockStabilized;

            OnChunkChanged.Raise(this);
        }

        public void Register(Feature feature)
        {
            FeatureMap[feature.Position] = feature;
            feature.transform.SetParent(transform, true);

            feature.OnFeaturedDestroyed += OnFeatureDestroyed;
            OnChunkChanged.Raise(this);
        }

        public void Register(Hazard hazard)
        {
            Hazards.Add(hazard);
            hazard.transform.SetParent(transform, true);

            hazard.OnHazardChanged += OnHazardChanged;
            hazard.OnHazardDestroyed += OnHazardDestroyed;

            OnChunkChanged.Raise(this);
        }

        public void Register(string spaceName)
        {
            if (!SpacesUsed.Contains(spaceName))
            {
                SpacesUsed.Add(spaceName);
            }
        }

        public void Register(EnemyHealth enemy)
        {
            Enemies.Add(enemy);
            enemy.transform.SetParent(transform, true);

            PositionTracker.Subscribe(enemy, OnEnemyPositionUpdate);

            enemy.OnActorDeath += Unregister;

            enemy.gameObject.SetActive(gameObject.activeInHierarchy);

            OnChunkChanged.Raise(this);
        }

        public void Register(ItemActor item)
        {
            Items.Add(item);
            item.OnItemDestroyed += OnItemDestroyed;
            OnChunkChanged.Raise(this);
        }

        public Block GetBlockForPosition(IntVector2 position)
        {
            BlockMap.TryGetValue(position, out var block);
            return block;
        }

        public void SetState(ChunkState newState)
        {
            if (State == ChunkState.Constructing)
            {
                // Make sure we are set correctly
                if (newState == ChunkState.Constructing)
                {
                    State = newState;
                }
                // The only transition out of constructing is to Ready
                else if (newState == ChunkState.Ready)
                {
                    State = ChunkState.Ready;
                }
                else
                {
                    State = ChunkState.Constructing;
                    _log.Warning($"Chunk is not ready to be {newState}.  Ignoring.");
                }
            }
            else if (State != ChunkState.Constructing)
            {
                if (newState == ChunkState.Constructing)
                {
                    throw new InvalidOperationException($"Chunk cannot be returned to `Constructing` from {State}.");
                }
                else
                {
                    State = newState;
                }
            }

            var isActive = State == ChunkState.Active;
            gameObject.SetActive(isActive);
            foreach (var enemy in Enemies)
            {
                enemy.SetActive(isActive);
            }

            if (State == ChunkState.Ready)
            {
                OnChunkReady.Raise(this);
            }
            else if (State == ChunkState.Active)
            {
                OnChunkActivated.Raise(this);
            }
        }

        protected List<IntVector2> GetEdgesReached(IntVector2 extentPoint)
        {
            var edgesReached = new List<IntVector2>();

            if (extentPoint.X <= BottomLeftCorner.X) edgesReached.Add(Vector2.left);
            if (extentPoint.Y <= BottomLeftCorner.Y) edgesReached.Add(Vector2.down);
            if (extentPoint.X >= TopRightCorner.X) edgesReached.Add(Vector2.right);
            if (extentPoint.Y >= TopRightCorner.Y) edgesReached.Add(Vector2.up);

            return edgesReached;
        }

        private void OnBlockDestroyed(Block block)
        {
            block.OnBlockCrumbled -= OnBlockCrumbled;
            block.OnBlockDestroyed -= OnBlockDestroyed;

            if (!BlockMap.Remove(block.Position)) _log.Info($"Attempted to destroy block, but could not find it at {block.Position}.");
            else _log.Info($"Block destroyed at {block.Position}.");

            OnChunkChanged.Raise(this);
        }

        private void OnBlockCrumbled(Block block)
        {
            if (!BlockMap.Remove(block.Position)) throw new Exception($"Attempted to crumble block, but could not find it at {block.Position}.");
            else _log.Info($"Block crumbled at {block.Position}.");

            OnChunkChanged.Raise(this);
        }

        private void OnBlockStabilized(Block block)
        {
            if (BlockMap.ContainsKey(block.Position)) throw new Exception($"Attempted to add block, but one already exists at {block.Position}!");
            else
            {
                BlockMap[block.Position] = block;
            }

            _log.Info($"Block stabilized at {block.Position}.");

            OnChunkChanged.Raise(this);
        }

        private void OnFeatureDestroyed(Feature feature)
        {

            feature.OnFeaturedDestroyed -= OnFeatureDestroyed;

            if (!FeatureMap.Remove(feature.Position)) _log.Info($"Attempted to destroy feature, but could not find it at {feature.Position}.");
            else _log.Info($"Feature destroyed at {feature.Position}.");

            OnChunkChanged.Raise(this);
        }

        private void OnHazardChanged(Hazard hazard)
        {
            if (!Hazards.Contains(hazard))
            {
                throw new InvalidOperationException($"Attempted to updated hazard `{hazard}` but it could not be found!");
            }
            else OnChunkChanged.Raise(this);
        }

        private void OnHazardDestroyed(Hazard hazard)
        {
            if (Hazards.Contains(hazard))
            {
                Hazards.Remove(hazard);
                _log.Info($"Hazard {hazard} has been removed.");
            }
            else throw new InvalidOperationException($"Attempted to remove hazard {hazard} but it could not be found!");

            hazard.OnHazardChanged -= OnHazardChanged;
            hazard.OnHazardDestroyed -= OnHazardDestroyed;

            OnChunkChanged.Raise(this);
        }

        private void OnItemDestroyed(ItemActor itemActor)
        {
            if (Items.Contains(itemActor))
            {
                Items.Remove(itemActor);
                _log.Info($"Item {itemActor} has been removed.");
            }
            else throw new InvalidOperationException($"Attempted to remove item {itemActor} but it could not be found!");

            itemActor.OnItemDestroyed -= OnItemDestroyed;

            OnChunkChanged.Raise(this);
        }

        private void Unregister(ActorHealth enemy)
        {
            Enemies.Remove(enemy as EnemyHealth);

            PositionTracker.Unsubscribe(enemy, OnEnemyPositionUpdate);

            enemy.OnActorDeath -= Unregister;
        }

        private void OnEnemyPositionUpdate(ITrackable trackedEnemy, PositionData oldPosition, PositionData newPosition)
        {
            var enemy = trackedEnemy as EnemyHealth;

            if (newPosition.Chunk != this)
            {
                Unregister(enemy);

                if (newPosition.Chunk != null)
                {
                    newPosition.Chunk.Register(enemy);
                }
                else if (newPosition.Builder != null)
                {
                    newPosition.Builder.AddEnemy(enemy);
                }
                else
                {
                    _log.Error($"Enemy {trackedEnemy} has left known bounds at location {newPosition.Position}.");
                }
            }
        }

        public override bool Equals(object obj)
        {
            var chunk = obj as Chunk;
            return chunk != null &&
                   base.Equals(obj) &&
                   EqualityComparer<Dictionary<IntVector2, Block>>.Default.Equals(BlockMap, chunk.BlockMap) &&
                   EqualityComparer<IntVector2>.Default.Equals(BottomLeftCorner, chunk.BottomLeftCorner) &&
                   EqualityComparer<IntVector2>.Default.Equals(TopRightCorner, chunk.TopRightCorner);
        }

        public override int GetHashCode()
        {
            var hashCode = 471642533;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Dictionary<IntVector2, Block>>.Default.GetHashCode(BlockMap);
            hashCode = hashCode * -1521134295 + EqualityComparer<IntVector2>.Default.GetHashCode(BottomLeftCorner);
            hashCode = hashCode * -1521134295 + EqualityComparer<IntVector2>.Default.GetHashCode(TopRightCorner);
            return hashCode;
        }

        protected static readonly Utilities.Debug.Log _log = new Utilities.Debug.Log("Chunk");

        public enum ChunkState
        {
            Constructing,
            Ready,
            Active,
            Inactive
        }
    }
}