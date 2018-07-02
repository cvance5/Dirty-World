using UnityEngine;

namespace WorldObjects.WorldGeneration
{
    public class BlockBuilder
    {
        private bool _exists;
        public IntVector2 WorldPosition { get; private set; }

        public BlockBuilder(IntVector2 worldPosition)
        {
            WorldPosition = worldPosition;
            _exists = true;
        }

        public BlockBuilder Remove()
        {
            _exists = false;
            return this;
        }

        public Block Build()
        {
            Block block = null;

            if (_exists)
            {
                block = Object.Instantiate(GameManager.Instance.Settings.Block, WorldPosition, Quaternion.identity).GetComponent<Block>();
                block.name = $"[{WorldPosition.X}, {WorldPosition.Y}]";
            }

            return block;
        }
    }
}