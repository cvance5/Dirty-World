using MathConcepts;
using UnityEngine;
using WorldObjects.Spaces;
using WorldObjects.WorldGeneration.EnemyGeneration;

namespace Tools.SpaceCrafting
{
    public class EnemySpawnCrafter : MonoBehaviour
    {
        public EnemyTypes Type;

        public EnemySpawn Build() => new EnemySpawn(new IntVector2(transform.position), Type);

        private void Awake() => OnValidate();

        private void OnValidate() => transform.position = new IntVector2(transform.position);
    }
}