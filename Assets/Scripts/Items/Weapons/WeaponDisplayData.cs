using UnityEngine;

namespace Items.Weapons
{
    public class WeaponDisplayData : ScriptableObject
    {
        public WeaponTypes Type = WeaponTypes.HarpoonGun;
#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly if we want Unity Serialization
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
    }
}