using System;
using System.Collections.Generic;
using UnityEngine;
using WorldObjects.Hazards;

namespace WorldObjects.WorldGeneration
{
    public class HazardLoader : Singleton<HazardLoader>
    {
#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly since we want it serialized by unity
        [Header("Hazards")]
        [SerializeField]
        public GameObject _spikeHazard = null;
#pragma warning restore IDE0044 // Add readonly modifier

        public static Hazard CreateHazard(HazardTypes type, IntVector2 worldPosition)
        {
            GameObject hazardObject;

            switch (type)
            {
                case HazardTypes.Spike: hazardObject = Instance._spikeHazard; break;
                default: throw new ArgumentException($"Unknown hazard type of {type}.");
            }

            hazardObject = Instantiate(hazardObject);
            hazardObject.transform.position = worldPosition;
            hazardObject.name = $"[{worldPosition.X}, {worldPosition.Y}]";

            var hazard = hazardObject.GetComponent<Hazard>();

            _log.ErrorIfNull(hazard, $"Hazard of type {type} has not been given a 'hazard' component.");

            return hazard;
        }

        public static Type ConvertToType(HazardTypes enumType) => _enumToType[enumType];
        public static HazardTypes ConvertToEnum(Type type) => _typeToEnum[type];

        private static readonly Dictionary<HazardTypes, Type> _enumToType = new Dictionary<HazardTypes, Type>()
        {
            { HazardTypes.None, null },
            { HazardTypes.Spike, typeof(SpikeHazard) }
        };

        private static readonly Dictionary<Type, HazardTypes> _typeToEnum = new Dictionary<Type, HazardTypes>()
        {
            { typeof(SpikeHazard), HazardTypes.Spike }
        };

        private static readonly Log _log = new Log("HazardLoader");
    }
}