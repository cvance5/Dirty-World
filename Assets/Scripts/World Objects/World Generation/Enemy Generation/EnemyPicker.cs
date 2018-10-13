using System.Collections.Generic;
using System.Linq;

namespace WorldObjects.WorldGeneration.EnemyGeneration
{
    public static class EnemyPicker
    {
        public static EnemyTypes RequestEnemy(EnemyRequestCriteria requestCriteria)
        {
            var enemy = EnemyTypes.None;

            foreach (var possibleEnemy in _requirementsForEnemy.OrderBy(requirement => UnityEngine.Random.value))
            {
                if (possibleEnemy.Value.Compare(requestCriteria))
                {
                    enemy = possibleEnemy.Key;
                    break;
                }
            }

            return enemy;
        }

        private static readonly Dictionary<EnemyTypes, EnemyRequirements> _requirementsForEnemy = new Dictionary<EnemyTypes, EnemyRequirements>()
        {
            { EnemyTypes.Maggot, new EnemyRequirements()
                {
                    Height = 1,
                    Length = 2
                }
            }
        };

        private class EnemyRequirements
        {
            public int Height;
            public int Length;

            public bool Compare(EnemyRequestCriteria criteria)
            {
                if (!criteria.HeightsAllowed?.IsInRange(Height) ?? false)
                {
                    return false;
                }

                if (!criteria.LengthsAllowed?.IsInRange(Length) ?? false)
                {
                    return false;
                }

                return true;
            }
        }
    }
}