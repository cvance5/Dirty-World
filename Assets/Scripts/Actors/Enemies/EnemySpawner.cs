using UnityEngine;

namespace Actors.Enemies
{
    public class EnemySpawner : Singleton<EnemySpawner>
    {
#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly since we want it serialized by unity
        [Header("Enemies")]
        [SerializeField]
        private GameObject _maggot = null;
#pragma warning restore IDE0044 // Add readonly modifier

        public static EnemyData SpawnEnemy(EnemyTypes type, IntVector2 worldPosition)
        {
            GameObject enemyObject;

            switch (type)
            {
                case EnemyTypes.Maggot: enemyObject = Instance._maggot; break;
                default: throw new System.ArgumentException($"Unknown enemy type of {type}.");
            }

            enemyObject = Instantiate(enemyObject);
            enemyObject.transform.position = worldPosition;

            var enemyData = enemyObject.GetComponent<EnemyData>();

            _log.ErrorIfNull(enemyData, $"Enemy of type {type} has not been given an 'enemy' component.");

            return enemyData;
        }

        private static readonly Log _log = new Log("EnemySpawner");
    }
}