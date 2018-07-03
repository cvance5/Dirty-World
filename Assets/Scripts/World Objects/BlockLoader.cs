using UnityEngine;

public class BlockLoader : Singleton<BlockLoader>
{
    public GameObject DirtBlock;
    public GameObject StoneBlock;

    public static GameObject CreateBlock(BlockTypes type, IntVector2 worldPosition)
    {
        GameObject blockObject;

        switch (type)
        {
            case BlockTypes.Dirt: blockObject = Instantiate(Instance.DirtBlock); break;
            case BlockTypes.Stone: blockObject = Instantiate(Instance.StoneBlock); break;
            default: throw new System.ArgumentException($"Unknown block type of {type}.");
        }

        blockObject.transform.position = worldPosition;
        blockObject.name = blockObject.name = $"[{worldPosition.X}, {worldPosition.Y}]";

        return blockObject;
    }
}
