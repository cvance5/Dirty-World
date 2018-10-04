using System.Collections.Generic;
using UnityEngine;

namespace Items.Weapons
{
    public class WeaponDisplayData : ScriptableObject
    {
        private static Dictionary<WeaponTypes, WeaponDisplayData> _registeredDisplays = new Dictionary<WeaponTypes, WeaponDisplayData>();

#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly if we want Unity Serialization
        [SerializeField]
        private WeaponTypes _type = WeaponTypes.HarpoonGun;
        [SerializeField]
        private string _name = null;
        public string Name => _name;
        [SerializeField]
        private Sprite _image = null;
        public Sprite Image => _image;
        [SerializeField]
        private string _description = null;
        public string Description => _description;
#pragma warning restore IDE0044 // Add readonly modifier

        // For ScriptableObjects, this call when an instance is created, or when the level is loaded that references them
        private void OnEnable()
        {
            Register(this);
        }

        private static void Register(WeaponDisplayData caller)
        {
            _registeredDisplays.Add(caller._type, caller);
        }

        public static WeaponDisplayData GetDisplay(WeaponTypes type)
        {
            if (_registeredDisplays.TryGetValue(type, out var weaponDisplayData))
            {
                return weaponDisplayData;
            }
            else throw new System.ArgumentException($"No registered weapon display data for a weapon of type {type}.");
        }
    }
}