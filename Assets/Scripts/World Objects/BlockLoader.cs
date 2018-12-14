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

    [Header("sourceTextures")]
    [SerializeField]
    private Texture2D _dirtTexture = null;
    [SerializeField]
    private Texture2D _stoneTexture = null;
#pragma warning restore IDE0044 // Add readonly modifier

    private const int TEXTURE_SCALE = 64;
    private static readonly Vector2 SPRITE_SIZE = new Vector2(TEXTURE_SCALE, TEXTURE_SCALE);
    private static readonly Vector2 SPRITE_ANCHOR = new Vector2(0.5f, 0.5f);

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

        if (type == BlockTypes.Dirt)
        {
            var spriteRenderer = blockObject.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = CreateBlockSprite(Instance._dirtTexture, worldPosition);
        }
        else if (type == BlockTypes.Stone)
        {
            var spriteRenderer = blockObject.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = CreateBlockSprite(Instance._stoneTexture, worldPosition);
        }

        var block = blockObject.GetComponent<Block>();
        blockObject.name = block.ObjectName;

        _log.ErrorIfNull(block, $"Block of type {type} has not been given a 'block' component.");

        return block;
    }

    public static Type ConvertToType(BlockTypes enumType) => _enumToType[enumType];
    public static BlockTypes ConvertToEnum(Type type) => _typeToEnum[type];

    private static Sprite CreateBlockSprite(Texture2D source, IntVector2 worldPosition)
    {
        var pixelPositionX = (worldPosition.X * TEXTURE_SCALE) % source.width;
        var pixelPositionY = (worldPosition.Y * TEXTURE_SCALE) % source.height;

        var pixelPosition = new Vector2(pixelPositionX, pixelPositionY);

        return Sprite.Create(source, new Rect(pixelPosition, SPRITE_SIZE), SPRITE_ANCHOR, TEXTURE_SCALE);
    }

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

    private static readonly Utilities.Debug.Log _log = new Utilities.Debug.Log("BlockLoader");
}