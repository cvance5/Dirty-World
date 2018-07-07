﻿using UnityEngine;

namespace WorldObjects.WorldGeneration
{
    public class ObjectLoader : Singleton<ObjectLoader>
    {
        [Header("Blocks")]
        public GameObject DirtBlock;
        public GameObject StoneBlock;
        public GameObject GoldBlock;

        [Header("Hazards")]
        public GameObject SpikeHazard;

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

            var block = blockObject.GetComponent<Block>();
            blockObject.name = block.GetObjectName();

            Log.ErrorIfNull(block, $"Block of type {type} has not been given a 'block' component.");

            return block;
        }

        public static Hazard CreateHazard(HazardTypes type, IntVector2 worldPosition)
        {
            GameObject hazardObject;

            switch (type)
            {
                case HazardTypes.Spike: hazardObject = Instance.SpikeHazard; break;
                default: throw new System.ArgumentException($"Unknown hazard type of {type}.");
            }

            hazardObject = Instantiate(hazardObject);
            hazardObject.transform.position = worldPosition;
            hazardObject.name = $"[{worldPosition.X}, {worldPosition.Y}]";

            var hazard = hazardObject.GetComponent<Hazard>();

            Log.ErrorIfNull(hazard, $"Hazard of type {type} has not been given a 'hazard' component.");

            return hazard;
        }
    }
}