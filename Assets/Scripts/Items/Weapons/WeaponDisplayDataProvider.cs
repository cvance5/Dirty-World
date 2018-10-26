using System.Collections.Generic;
using UnityEngine;

namespace Items.Weapons
{
    public class WeaponDisplayDataProvider : Singleton<WeaponDisplayDataProvider>
    {
#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly since we want it serialized by unity
        [SerializeField]
        private List<WeaponDisplayData> _weaponDisplayData = new List<WeaponDisplayData>();
#pragma warning restore IDE0044 // Add readonly modifier

        public static WeaponDisplayData GetDisplay(WeaponTypes type)
        {
            var weaponDisplayData = Instance._weaponDisplayData.Find(wdd => wdd.Type == type);

            if (weaponDisplayData == null)
            {
                throw new System.ArgumentException($"No weapon display data registered for type {type}.");
            }

            return weaponDisplayData;
        }
    }
}