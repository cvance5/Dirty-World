using System;
using System.Collections.Generic;
using UnityEngine;

namespace WorldObjects
{
    public class Chunk : MonoBehaviour, IBoundary
    {
        public IntVector2 Position => new IntVector2(transform.position);

        private List<Space> _spaces = new List<Space>();
        private Dictionary<IntVector2, Block> _blockMap = new Dictionary<IntVector2, Block>();
        private Dictionary<IntVector2, List<Space>> _spacesOverlappingEdges = new Dictionary<IntVector2, List<Space>>()
        {
            { Vector2.up, new List<Space>() },
            { Vector2.right, new List<Space>() },
            { Vector2.down, new List<Space>() },
            { Vector2.left, new List<Space>() }
        };

        private IntVector2 _bottomLeftCorner;
        private IntVector2 _topRightCorner;

        public void AssignExtents(IntVector2 bottomLeftCorner, IntVector2 topRightCorner)
        {
            _bottomLeftCorner = bottomLeftCorner;
            _topRightCorner = topRightCorner;
        }

        public void Register(Block block)
        {
            _blockMap[block.Position] = block;
            block.transform.SetParent(transform, true);

            block.OnDestroyed += OnBlockDestroyed;
            block.OnCrumbled += OnBlockCrumbled;
            block.OnStabilized += OnBlockStabilized;
        }

        public void Register(Space space)
        {
            _spaces.Add(space);

            List<IntVector2> edgesReached = new List<IntVector2>();

            foreach (var extentPoint in space.Extents)
            {
                if (!Contains(extentPoint))
                {
                    if (extentPoint.X < _bottomLeftCorner.X) edgesReached.Add(Vector2.left);
                    if (extentPoint.Y < _bottomLeftCorner.Y) edgesReached.Add(Vector2.down);
                    if (extentPoint.X > _bottomLeftCorner.X) edgesReached.Add(Vector2.right);
                    if (extentPoint.Y > _bottomLeftCorner.Y) edgesReached.Add(Vector2.up);
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
            _blockMap.TryGetValue(position, out block);
            return block;
        }

        public Space GetSpaceForPosition(IntVector2 position)
        {
            if (!Contains(position)) throw new ArgumentOutOfRangeException($"Chunk does not contains {position}.");

            foreach (var space in _spaces)
            {
                if (space.Contains(position)) return space;
            }
            return null;
        }

        public List<Space> GetSpacesReachingEdge(IntVector2 edge) => _spacesOverlappingEdges[edge];

        public bool Contains(IntVector2 position) =>
            position.X >= _bottomLeftCorner.X &&
            position.Y >= _bottomLeftCorner.Y &&
            position.X <= _topRightCorner.X &&
            position.Y <= _topRightCorner.Y;

        private void OnBlockDestroyed(Block block)
        {
            block.OnCrumbled -= OnBlockCrumbled;
            block.OnDestroyed -= OnBlockDestroyed;

            if (!_blockMap.Remove(block.Position)) Log.Info($"Attempted to destroy block, but could not find it at {block.Position}.");
            else Log.Info($"Block destroyed at {block.Position}.");
        }

        private void OnBlockCrumbled(Block block)
        {
            if (!_blockMap.Remove(block.Position)) throw new Exception($"Attempted to crumble block, but could not find it at {block.Position}.");
            else Log.Info($"Block crumbled at {block.Position}.");
        }

        private void OnBlockStabilized(Block block)
        {
            if (_blockMap.ContainsKey(block.Position)) throw new Exception($"Attempted to add block, but one already exists at {block.Position}!");
            else
            {
                _blockMap[block.Position] = block;
            }

            Log.Info($"Block stabilized at {block.Position}.");
        }

        public override string ToString() => $"Chunk from {_bottomLeftCorner} to {_topRightCorner}.";

        public override bool Equals(object obj)
        {
            var chunk = obj as Chunk;
            return chunk != null &&
                   base.Equals(obj) &&
                   EqualityComparer<IntVector2>.Default.Equals(Position, chunk.Position) &&
                   EqualityComparer<List<Space>>.Default.Equals(_spaces, chunk._spaces) &&
                   EqualityComparer<Dictionary<IntVector2, Block>>.Default.Equals(_blockMap, chunk._blockMap) &&
                   EqualityComparer<Dictionary<IntVector2, List<Space>>>.Default.Equals(_spacesOverlappingEdges, chunk._spacesOverlappingEdges) &&
                   EqualityComparer<IntVector2>.Default.Equals(_bottomLeftCorner, chunk._bottomLeftCorner) &&
                   EqualityComparer<IntVector2>.Default.Equals(_topRightCorner, chunk._topRightCorner);
        }

        public override int GetHashCode()
        {
            var hashCode = 471642533;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<IntVector2>.Default.GetHashCode(Position);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<Space>>.Default.GetHashCode(_spaces);
            hashCode = hashCode * -1521134295 + EqualityComparer<Dictionary<IntVector2, Block>>.Default.GetHashCode(_blockMap);
            hashCode = hashCode * -1521134295 + EqualityComparer<Dictionary<IntVector2, List<Space>>>.Default.GetHashCode(_spacesOverlappingEdges);
            hashCode = hashCode * -1521134295 + EqualityComparer<IntVector2>.Default.GetHashCode(_bottomLeftCorner);
            hashCode = hashCode * -1521134295 + EqualityComparer<IntVector2>.Default.GetHashCode(_topRightCorner);
            return hashCode;
        }
    }
}

