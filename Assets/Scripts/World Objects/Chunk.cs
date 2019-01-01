using Data;
using Items.ItemActors;
using System;
using System.Collections.Generic;
using UnityEngine;
using WorldObjects.Actors;
using WorldObjects.Actors.Enemies;
using WorldObjects.Blocks;
using WorldObjects.Hazards;
using Space = WorldObjects.Spaces.Space;

namespace WorldObjects
{
    public class Chunk : MonoBehaviour, ITrackable, IBoundary
    {
        public static SmartEvent<Chunk> OnChunkChanged = new SmartEvent<Chunk>();

        public IntVector2 Position => new IntVector2(transform.position);
        public IntVector2 BottomLeftCorner { get; protected set; }
        public IntVector2 TopRightCorner { get; protected set; }

        public List<Space> Spaces { get; private set; } = new List<Space>();
        public List<Hazard> Hazards { get; private set; } = new List<Hazard>();
        public List<EnemyData> Enemies { get; private set; } = new List<EnemyData>();
        public List<ItemActor> Items { get; private set; } = new List<ItemActor>();

        public Dictionary<IntVector2, Block> BlockMap { get; private set; } = new Dictionary<IntVector2, Block>();

        private static World _world;

        public static void AssignWorld(World world) => _world = world;

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

        public void Register(Space space)
        {
            if (Spaces.Contains(space)) return;

            Spaces.Add(space);

            var edgesReached = new List<IntVector2>();

            foreach (var extentPoint in space.Extents)
            {
                if (!Contains(extentPoint))
                {
                    edgesReached.AddRange(GetEdgesReached(extentPoint));
                }

                // We are already overlapping all edges with 
                // the previous extents, so don't check the rest.
                if (edgesReached.Count == 4) break;
            }

            foreach (var dir in edgesReached)
            {
                var neighbor = _world.GetChunkNeighbor(Position, dir);

                if (neighbor != null)
                {
                    throw new InvalidOperationException($"A space overlaps with an existing chunk! Chunk {neighbor} and space {space}.");
                }
                else
                {
                    var blueprint = _world.GetBlueprintNeighbor(Position, dir);
                    blueprint.Register(space);
                }
            }

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

        public void Register(EnemyData enemy)
        {
            Enemies.Add(enemy);

            PositionTracker.Subscribe(enemy, OnEnemyPositionUpdate);

            enemy.OnActorDeath += Unregister;

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

        public Space GetSpaceForPosition(IntVector2 position)
        {
            if (!Contains(position)) throw new ArgumentOutOfRangeException($"Chunk does not contains {position}.");

            foreach (var space in Spaces)
            {
                if (space.Contains(position)) return space;
            }
            return null;
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

        private void Unregister(ActorData enemy)
        {
            Enemies.Remove(enemy as EnemyData);

            PositionTracker.Unsubscribe(enemy, OnEnemyPositionUpdate);

            enemy.OnActorDeath -= Unregister;
        }

        private void OnEnemyPositionUpdate(ITrackable trackedEnemy, PositionData oldPosition, PositionData newPosition)
        {
            var enemy = trackedEnemy as EnemyData;

            if (newPosition.Chunk != this)
            {
                Unregister(enemy);

                if (newPosition.Chunk != null)
                {
                    newPosition.Chunk.Register(enemy);
                }
                else
                {
                    var edgesPassed = GetEdgesReached(newPosition.Position);
                    IntVector2 chunkDir = Vector2.zero;

                    // Merge edges passed to determine a ordinal or cardinal direction
                    foreach (var edgePassed in edgesPassed)
                    {
                        chunkDir += edgePassed;
                    }

                    var blueprintForChunk = _world.GetBlueprintForPosition(Position + (chunkDir * _world.ChunkSize));
                    blueprintForChunk.Register(enemy);
                }
            }
        }

        public override bool Equals(object obj)
        {
            var chunk = obj as Chunk;
            return chunk != null &&
                   base.Equals(obj) &&
                   EqualityComparer<List<Space>>.Default.Equals(Spaces, chunk.Spaces) &&
                   EqualityComparer<Dictionary<IntVector2, Block>>.Default.Equals(BlockMap, chunk.BlockMap) &&
                   EqualityComparer<IntVector2>.Default.Equals(BottomLeftCorner, chunk.BottomLeftCorner) &&
                   EqualityComparer<IntVector2>.Default.Equals(TopRightCorner, chunk.TopRightCorner);
        }

        public override int GetHashCode()
        {
            var hashCode = 471642533;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<List<Space>>.Default.GetHashCode(Spaces);
            hashCode = hashCode * -1521134295 + EqualityComparer<Dictionary<IntVector2, Block>>.Default.GetHashCode(BlockMap);
            hashCode = hashCode * -1521134295 + EqualityComparer<IntVector2>.Default.GetHashCode(BottomLeftCorner);
            hashCode = hashCode * -1521134295 + EqualityComparer<IntVector2>.Default.GetHashCode(TopRightCorner);
            return hashCode;
        }

        protected static readonly Utilities.Debug.Log _log = new Utilities.Debug.Log("Chunk");
    }
}