using System;
using System.Collections.Generic;
using UnityEngine;

namespace WorldObjects
{
    public class Chunk : MonoBehaviour, IBoundary, ITrackable
    {
        public SmartEvent<Chunk> OnChunkChanged = new SmartEvent<Chunk>();

        public IntVector2 Position => new IntVector2(transform.position);
        public IntVector2 BottomLeftCorner { get; private set; }
        public IntVector2 TopRightCorner { get; private set; }

        public List<Space> Spaces { get; private set; } = new List<Space>();
        public List<Hazard> Hazards { get; private set; } = new List<Hazard>();
        public Dictionary<IntVector2, Block> BlockMap { get; private set; } = new Dictionary<IntVector2, Block>();

        private Dictionary<IntVector2, List<Space>> _spacesOverlappingEdges = new Dictionary<IntVector2, List<Space>>()
        {
            { Vector2.up, new List<Space>() },
            { Vector2.right, new List<Space>() },
            { Vector2.down, new List<Space>() },
            { Vector2.left, new List<Space>() }
        };

        public void AssignExtents(IntVector2 bottomLeftCorner, IntVector2 topRightCorner)
        {
            BottomLeftCorner = bottomLeftCorner;
            TopRightCorner = topRightCorner;
        }

        public void Register(Block block)
        {
            BlockMap[block.Position] = block;
            block.transform.SetParent(transform, true);

            block.OnBlockDestroyed += OnBlockDestroyed;
            block.OnBlockCrumbled += OnBlockCrumbled;
            block.OnBlockStabilized += OnBlockStabilized;
        }

        public void Register(Hazard hazard)
        {
            var anchorPos = hazard.AnchoringPosition;

            if (anchorPos != null)
            {
                Block anchor = null;
                BlockMap.TryGetValue(anchorPos, out anchor);
                hazard.SetAnchor(anchor);
            }

            // The hazard may have destroyed itself because of its 
            // anchoring situation.
            if (hazard != null)
            {
                Hazards.Add(hazard);
                hazard.transform.SetParent(transform, true);
                hazard.OnHazardDestroyed += OnHazardRemoved;
            }
        }

        public void Register(Space space)
        {
            Spaces.Add(space);

            List<IntVector2> edgesReached = new List<IntVector2>();

            foreach (var extentPoint in space.Extents)
            {
                if (!Contains(extentPoint))
                {
                    if (extentPoint.X < BottomLeftCorner.X) edgesReached.Add(Vector2.left);
                    if (extentPoint.Y < BottomLeftCorner.Y) edgesReached.Add(Vector2.down);
                    if (extentPoint.X > BottomLeftCorner.X) edgesReached.Add(Vector2.right);
                    if (extentPoint.Y > BottomLeftCorner.Y) edgesReached.Add(Vector2.up);
                }

                // We are already overlapping all edges with 
                // the previous extents, so don't check the rest.
                if (edgesReached.Count == 4) break;
            }

            if (edgesReached.Count > 0)
            {
                foreach (var edgeReached in edgesReached)
                {
                    _spacesOverlappingEdges[edgeReached].Add(space);
                }
            }
        }

        public Block GetBlockForPosition(IntVector2 position)
        {
            Block block = null;
            BlockMap.TryGetValue(position, out block);
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

        public List<Space> GetSpacesReachingEdge(IntVector2 edge) => _spacesOverlappingEdges[edge];

        public bool Contains(IntVector2 position) =>
            position.X >= BottomLeftCorner.X &&
            position.Y >= BottomLeftCorner.Y &&
            position.X <= TopRightCorner.X &&
            position.Y <= TopRightCorner.Y;

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

        private void OnHazardRemoved(Hazard hazard)
        {
            if (Hazards.Contains(hazard))
            {
                Hazards.Remove(hazard);
                _log.Info($"Hazard {hazard} has been removed.");
            }
            else throw new Exception($"Attempted to remove hazard {hazard} but it could not be found!");

            hazard.OnHazardDestroyed -= OnHazardRemoved;

            OnChunkChanged.Raise(this);
        }

        public override bool Equals(object obj)
        {
            var chunk = obj as Chunk;
            return chunk != null &&
                   base.Equals(obj) &&
                   EqualityComparer<List<Space>>.Default.Equals(Spaces, chunk.Spaces) &&
                   EqualityComparer<Dictionary<IntVector2, Block>>.Default.Equals(BlockMap, chunk.BlockMap) &&
                   EqualityComparer<Dictionary<IntVector2, List<Space>>>.Default.Equals(_spacesOverlappingEdges, chunk._spacesOverlappingEdges) &&
                   EqualityComparer<IntVector2>.Default.Equals(BottomLeftCorner, chunk.BottomLeftCorner) &&
                   EqualityComparer<IntVector2>.Default.Equals(TopRightCorner, chunk.TopRightCorner);
        }

        public override int GetHashCode()
        {
            var hashCode = 471642533;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<List<Space>>.Default.GetHashCode(Spaces);
            hashCode = hashCode * -1521134295 + EqualityComparer<Dictionary<IntVector2, Block>>.Default.GetHashCode(BlockMap);
            hashCode = hashCode * -1521134295 + EqualityComparer<Dictionary<IntVector2, List<Space>>>.Default.GetHashCode(_spacesOverlappingEdges);
            hashCode = hashCode * -1521134295 + EqualityComparer<IntVector2>.Default.GetHashCode(BottomLeftCorner);
            hashCode = hashCode * -1521134295 + EqualityComparer<IntVector2>.Default.GetHashCode(TopRightCorner);
            return hashCode;
        }

        protected static readonly Log _log = new Log("Chunk");
    }
}