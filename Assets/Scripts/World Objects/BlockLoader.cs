using UnityEngine;

namespace WorldObjects.WorldGeneration
{
    public class BlockLoader : Singleton<BlockLoader>
    {
        public GameObject DirtBlock;
        public GameObject StoneBlock;
        public GameObject GoldBlock;

        public static Block CreateBlock(BlockTypes type, IntVector2 worldPosition)
        {
            GameObject blockObject;

            switch (type)
            {
                case BlockTypes.Dirt: blockObject = Instance.DirtBlock; break;
                case BlockTypes.Stone: blockObject = Instance.StoneBlock; break;
                case BlockTypes.Gold: blockObject = Instance.GoldBlock; break;
                default: throw new System.ArgumentException($"Unknown block type of {type}.");
            }

            blockObject = Instantiate(blockObject);
            blockObject.transform.position = worldPosition;
            blockObject.name = $"[{worldPosition.X}, {worldPosition.Y}]";

            var block = blockObject.GetComponent<Block>();

            Log.ErrorIfNull(block, $"Block of type {type} has not been given a 'block' component.");

            return block;
        }
    }
}