using System.Collections.Generic;
using System.Linq;

namespace WorldObjects.WorldGeneration.EnemyGeneration
{
    public static class EnemyPicker
    {
        public static int DetermineRiskPoints(int chunkDepth, int chunkRemoteness)
        {
            var randomizedValue = UnityEngine.Random.Range(-5, 5);
            return chunkDepth + chunkRemoteness + randomizedValue;
        }

        public static List<EnemyTypes> RequestEnemies(int riskPoints, EnemyRequestCriteria requestCriteria)
        {
            var enemies = new List<EnemyTypes>();

            while (riskPoints > 0)
            {
                var foundSomething = false;

                // Suffle all enemies, taking only ones we can afford
                var possibleEnemies = _requirementsForEnemy
                                      .Where(requirement => requirement.Value.RiskPointCost <= riskPoints)
                                      .OrderBy(requirement => UnityEngine.Random.value);

                foreach (var possibleEnemy in possibleEnemies)
                {
                    // If this random enemy works, save it, pay the bill, and reshuffle
                    if (possibleEnemy.Value.Compare(requestCriteria))
                    {
                        enemies.Add(possibleEnemy.Key);
                        riskPoints -= possibleEnemy.Value.RiskPointCost;
                        foundSomething = true;
                        break;
                    }
                }

                if (!foundSomething) break; // Nothing left that meets our criteria
            }

            return enemies;
        }

        private static readonly Dictionary<EnemyTypes, EnemyRequirements> _requirementsForEnemy = new Dictionary<EnemyTypes, EnemyRequirements>()
        {
            {
                EnemyTypes.Maggot, new EnemyRequirements()
                {
                    RiskPointCost = 3,

                    Height = 1,
                    Length = 2
                }
            }
        };

        private class EnemyRequirements
        {
            public int RiskPointCost;

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