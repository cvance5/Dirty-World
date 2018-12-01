using UnityEngine;

namespace WorldObjects.Actors.Enemies.Maggot
{
    public class MaggotData : EnemyData
    {
        public override string ObjectName => "Maggot";

#pragma warning disable IDE0044 // Add readonly modifier, Unity serialization requires it not be readonly
        [SerializeField]
        private MaggotDeathExplosion _deathExplosion = null;
#pragma warning restore IDE0044 // Add readonly modifier

        public override void Hit(int damage, int force)
        {
            ApplyDamage(damage + (force / 2));
            OnHit.Raise(this, damage, force);
        }

        protected override void OnDamage() { }

        protected override void OnDeath()
        {
            Instantiate(_deathExplosion.gameObject, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}