using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities.UnitySerialization;
using WorldObjects.Features;
using WorldObjects.WorldGeneration.FeatureGeneration;

namespace WorldObjects.Construction
{
    public class FeatureLoader : Singleton<FeatureLoader>
    {
#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly since we want it serialized by unity
        [Header("Features")]
        [SerializeField]
        private FeatureTypesGameObjectDictionary _features = new FeatureTypesGameObjectDictionary();
#pragma warning restore IDE0044 // Add readonly modifier

        private void Awake() => DontDestroyOnLoad(gameObject);

        public static Feature CreateFeature(FeatureTypes type, IntVector2 worldPosition)
        {
            if (!Instance._features.TryGetValue(type, out var featureObject))
            {
                _log.Error($"Could not find an object for featureType `{type}.");
            }

            featureObject = Instantiate(featureObject);
            featureObject.transform.position = worldPosition;

            var feature = featureObject.GetComponent<Feature>();
            featureObject.name = feature.ObjectName;

            _log.ErrorIfNull(feature, $"Block of type {type} has not been given a 'feature' component.");

            return feature;
        }

        public static Type ConvertToType(FeatureTypes enumType) => _enumToType[enumType];
        public static FeatureTypes ConvertToEnum(Type type) => _typeToEnum[type];

        private static readonly Dictionary<FeatureTypes, Type> _enumToType = new Dictionary<FeatureTypes, Type>()
        {
            { FeatureTypes.None, null },
            { FeatureTypes.WallLight, typeof(WallLight) }
        };

        private static readonly Dictionary<Type, FeatureTypes> _typeToEnum = new Dictionary<Type, FeatureTypes>()
        {
            { typeof(WallLight), FeatureTypes.WallLight }
        };

        private static readonly Utilities.Debug.Log _log = new Utilities.Debug.Log("FeatureLoader");
    }
}