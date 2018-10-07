using UnityEngine;

namespace Actors.Enemies.Maggot
{
    public class MaggotData : EnemyData, IHittable
    {
        public SmartEvent<int, int> OnHit { get; set; } = new SmartEvent<int, int>();

#pragma warning disable IDE0044 // Add readonly modifier, Unity serialization requires it not be readonly
        [SerializeField]
        private MaggotDeathExplosion _deathExplosion = null;
#pragma warning restore IDE0044 // Add readonly modifier

        public void Hit(int damage, int force)
        {
            ApplyDamage(damage + (force / 2));
            OnHit.Raise(damage, force);
        }

        protected override void OnDamage() { }

        protected override void OnDeath()
        {
            Instantiate(_deathExplosion.gameObject, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}