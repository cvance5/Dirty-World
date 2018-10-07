using System;
using System.Collections.Generic;
using UnityEngine;
using WorldObjects.Blocks;

public class BlockLoader : Singleton<BlockLoader>
{
#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly since we want it serialized by unity
    [Header("Blocks")]
    [SerializeField]
    private GameObject _dirtBlock = null;
    [SerializeField]
    private GameObject _stoneBlock = null;
    [SerializeField]
    private GameObject _copperBlock = null;
    [SerializeField]
    private GameObject _silverBlock = null;
    [SerializeField]
    private GameObject _goldBlock = null;
    [SerializeField]
    private GameObject _platinumBlock = null;
#pragma warning restore IDE0044 // Add readonly modifier

    public static Block CreateBlock(BlockTypes type, IntVector2 worldPosition)
    {
        GameObject blockObject;

        switch (type)
        {
            case BlockTypes.Dirt: blockObject = Instance._dirtBlock; break;
            case BlockTypes.Stone: blockObject = Instance._stoneBlock; break;
            case BlockTypes.Copper: blockObject = Instance._copperBlock; break;
            case BlockTypes.Silver: blockObject = Instance._silverBlock; break;
            case BlockTypes.Gold: blockObject = Instance._goldBlock; break;
            case BlockTypes.Platinum: blockObject = Instance._platinumBlock; break;
            default: throw new ArgumentException($"Unknown block type of {type}.");
        }

        blockObject = Instantiate(blockObject);
        blockObject.transform.position = worldPosition;

        var block = blockObject.GetComponent<Block>();
        blockObject.name = block.ObjectName;

        _log.ErrorIfNull(block, $"Block of type {type} has not been given a 'block' component.");

        return block;
    }

    public static Type ConvertToType(BlockTypes enumType) => _enumToType[enumType];
    public static BlockTypes ConvertToEnum(Type type) => _typeToEnum[type];

    private static readonly Dictionary<BlockTypes, Type> _enumToType = new Dictionary<BlockTypes, Type>()
    {
        { BlockTypes.None, null },
        { BlockTypes.Dirt, typeof(DirtBlock) },
        { BlockTypes.Stone, typeof(StoneBlock) },
        { BlockTypes.Copper, typeof(CopperBlock) },
        { BlockTypes.Silver, typeof(SilverBlock) },
        { BlockTypes.Gold, typeof(GoldBlock) },
        { BlockTypes.Platinum, typeof(PlatinumBlock) }
    };

    private static readonly Dictionary<Type, BlockTypes> _typeToEnum = new Dictionary<Type, BlockTypes>()
    {
        { typeof(DirtBlock), BlockTypes.Dirt },
        { typeof(StoneBlock), BlockTypes.Stone },
        { typeof(CopperBlock), BlockTypes.Copper },
        { typeof(SilverBlock), BlockTypes.Silver },
        { typeof(GoldBlock), BlockTypes.Gold },
        { typeof(PlatinumBlock), BlockTypes.Platinum }
    };

    private static readonly Log _log = new Log("BlockLoader");
}