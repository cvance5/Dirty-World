using System;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    private Dictionary<IntVector2, Block> _blockMap = new Dictionary<IntVector2, Block>();

    public void Register(Block block)
    {
        _blockMap[block.Position] = block;
        block.transform.SetParent(transform, true);

        block.OnDestroy += OnBlockDestroyed;
        block.OnCrumble += OnBlockCrumbled;
        block.OnStabilize += OnBlockStabilized;
    }

    private void OnBlockDestroyed(Block block)
    {
        foreach (var dir in _cardinals)
        {
            Block neighbor;
            if (_blockMap.TryGetValue(block.Position + dir, out neighbor))
            {
                neighbor.ApplyForce(25);
            }
        }

        block.OnCrumble -= OnBlockCrumbled;
        block.OnDestroy -= OnBlockDestroyed;

        if (!_blockMap.Remove(block.Position)) Log.Warning($"Attempted to destroy block, but could now find it.");

        Log.Info($"Block destroyed at {block.Position}.");
    }

    private void OnBlockCrumbled(Block block)
    {
        foreach (var dir in _cardinals)
        {
            Block neighbor;
            if (_blockMap.TryGetValue(block.Position + dir, out neighbor))
            {
                neighbor.ApplyForce(25);
            }
        }

        if (!_blockMap.Remove(block.Position)) throw new Exception($"Attempted to crumble block, but could now find it.");

        Log.Info($"Block crumbled at {block.Position}.");
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

    private static readonly List<Vector2> _cardinals = new List<Vector2>()
    {
        Vector2.up,
        Vector2.left,
        Vector2.down,
        Vector2.right
    };
}
