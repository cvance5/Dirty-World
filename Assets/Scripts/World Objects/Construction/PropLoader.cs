using MathConcepts;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities.UnitySerialization;
using WorldObjects.Props;
using WorldObjects.WorldGeneration.PropGeneration;

namespace WorldObjects.Construction
{
    public class PropLoader : Singleton<PropLoader>
    {
#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly since we want it serialized by unity
        [Header("props")]
        [SerializeField]
        private PropTypesGameObjectDictionary _props = new PropTypesGameObjectDictionary();
#pragma warning restore IDE0044 // Add readonly modifier

        private void Awake() => DontDestroyOnLoad(gameObject);

        public static Prop CreateProp(PropTypes type, IntVector2 worldPosition)
        {
            if (!Instance._props.TryGetValue(type, out var propObject))
            {
                _log.Error($"Could not find an object for propType `{type}.");
            }

            propObject = Instantiate(propObject);
            propObject.transform.position = worldPosition;

            var prop = propObject.GetComponent<Prop>();
            propObject.name = prop.ObjectName;

            _log.ErrorIfNull(prop, $"Block of type {type} has not been given a 'Prop' component.");

            return prop;
        }

        public static Type ConvertToType(PropTypes enumType) => _enumToType[enumType];
        public static PropTypes ConvertToEnum(Type type) => _typeToEnum[type];

        private static readonly Dictionary<PropTypes, Type> _enumToType = new Dictionary<PropTypes, Type>()
        {
            { PropTypes.None, null },
            { PropTypes.WallLight, typeof(WallLight) }
        };

        private static readonly Dictionary<Type, PropTypes> _typeToEnum = new Dictionary<Type, PropTypes>()
        {
            { typeof(WallLight), PropTypes.WallLight }
        };

        private static readonly Utilities.Debug.Log _log = new Utilities.Debug.Log("PropLoader");
    }
}