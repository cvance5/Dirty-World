using System;
using System.Collections.Generic;
using UnityEngine;
using WorldObjects.Actors.Enemies;
using WorldObjects.Actors.Enemies.Maggot;

namespace WorldObjects.WorldGeneration.EnemyGeneration
{
    public class EnemySpawner : Singleton<EnemySpawner>
    {
#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly since we want it serialized by unity
        [Header("Enemies")]
        [SerializeField]
        private GameObject _maggot = null;
#pragma warning restore IDE0044 // Add readonly modifier

        public static EnemyHealth SpawnEnemy(EnemyTypes type, IntVector2 worldPosition)
        {
            GameObject enemyObject;

            switch (type)
            {
                case EnemyTypes.Maggot: enemyObject = Instance._maggot; break;
                default: throw new ArgumentException($"Unknown enemy type of {type}.");
            }

            enemyObject = Instantiate(enemyObject);
            enemyObject.transform.position = worldPosition;

            var enemyData = enemyObject.GetComponent<EnemyHealth>();

            _log.ErrorIfNull(enemyData, $"Enemy of type {type} has not been given an 'enemy' component.");

            return enemyData;
        }

        public static Type ConvertToType(EnemyTypes enumType) => _enumToType[enumType];
        public static EnemyTypes ConvertToEnum(Type type) => _typeToEnum[type];

        private static readonly Dictionary<EnemyTypes, Type> _enumToType = new Dictionary<EnemyTypes, Type>()
        {
            { EnemyTypes.None, null },
            { EnemyTypes.Maggot, typeof(MaggotHealth) }
        };

        private static readonly Dictionary<Type, EnemyTypes> _typeToEnum = new Dictionary<Type, EnemyTypes>()
        {
            { typeof(MaggotHealth), EnemyTypes.Maggot }
        };

        private static readonly Utilities.Debug.Log _log = new Utilities.Debug.Log("EnemySpawner");
    }
}