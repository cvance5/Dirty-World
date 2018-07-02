using System;
using System.Collections.Generic;
using UnityEngine;

namespace WorldObjects
{
    public class Chunk : MonoBehaviour, IBoundary
    {
        private Dictionary<IntVector2, Block> _blockMap = new Dictionary<IntVector2, Block>();
        private List<Space> _spaces = new List<Space>();

        private IntVector2 _bottomLeftCorner;
        private IntVector2 _topRightCorner;

        public void Register(Block block)
        {
            _blockMap[block.Position] = block;
            block.transform.SetParent(transform, true);

            if (_bottomLeftCorner == null ||
                block.Position.X < _bottomLeftCorner.X ||
                block.Position.Y < _bottomLeftCorner.Y)
            {
                _bottomLeftCorner = block.Position;
            }
            if (_topRightCorner == null ||
                block.Position.X > _topRightCorner.X ||
                block.Position.Y > _topRightCorner.Y)
            {
                _topRightCorner = block.Position;
            }

            block.OnDestroy += OnBlockDestroyed;
            block.OnCrumble += OnBlockCrumbled;
            block.OnStabilize += OnBlockStabilized;
        }

        public void Register(Space space)
        {
            _spaces.Add(space);
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

        public bool Contains(IntVector2 position) =>
            position.X >= _bottomLeftCorner.X &&
            position.Y >= _bottomLeftCorner.Y &&
            position.X <= _topRightCorner.X &&
            position.Y <= _topRightCorner.Y;

        private void OnBlockDestroyed(Block block)
        {
            foreach (var dir in _neighborPositions)
            {
                Block neighbor;
                if (_blockMap.TryGetValue(block.Position + dir, out neighbor))
                {
                    neighbor.ApplyForce(25);
                }
            }

            block.OnCrumble -= OnBlockCrumbled;
            block.OnDestroy -= OnBlockDestroyed;

            if (!_blockMap.Remove(block.Position)) Log.Warning($"Attempted to destroy block, but could not find it at {block.Position}.");
            else Log.Info($"Block destroyed at {block.Position}.");
        }

        private void OnBlockCrumbled(Block block)
        {
            foreach (var dir in _neighborPositions)
            {
                Block neighbor;
                if (_blockMap.TryGetValue(block.Position + dir, out neighbor))
                {
                    neighbor.ApplyForce(25);
                }
            }

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

        private static readonly List<IntVector2> _neighborPositions = new List<IntVector2>()
        {
            Vector2.up,
            Vector2.left,
            Vector2.down,
            Vector2.right
        };

        public override string ToString() => $"Chunk from {_bottomLeftCorner} to {_topRightCorner}.";
    }
}

