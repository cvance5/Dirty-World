using UnityEngine;

namespace WorldGeneration
{
    public class BlockBuilder
    {
        private bool _exists;
        public Vector2 WorldPosition { get; private set; }

        public BlockBuilder(Vector2 worldPosition)
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
                block.name = $"[{WorldPosition.x}, {WorldPosition.y}]";
            }

            return block;
        }
    }
}