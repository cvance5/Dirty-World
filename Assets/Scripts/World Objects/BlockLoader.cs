using System;
using System.Collections.Generic;
using UnityEngine;
using WorldObjects;
using WorldObjects.Blocks;

public class BlockLoader : Singleton<BlockLoader>
{
    [Header("Blocks")]
    public GameObject DirtBlock;
    public GameObject StoneBlock;
    public GameObject CopperBlock;
    public GameObject SilverBlock;
    public GameObject GoldBlock;
    public GameObject PlatinumBlock;

    public static Block CreateBlock(BlockTypes type, IntVector2 worldPosition)
    {
        GameObject blockObject;

        switch (type)
        {
            case BlockTypes.Dirt: blockObject = Instance.DirtBlock; break;
            case BlockTypes.Stone: blockObject = Instance.StoneBlock; break;
            case BlockTypes.Copper: blockObject = Instance.CopperBlock; break;
            case BlockTypes.Silver: blockObject = Instance.SilverBlock; break;
            case BlockTypes.Gold: blockObject = Instance.GoldBlock; break;
            case BlockTypes.Platinum: blockObject = Instance.PlatinumBlock; break;
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