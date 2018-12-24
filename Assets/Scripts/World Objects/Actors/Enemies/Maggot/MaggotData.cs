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

        private MaggotCollider _collider;

        protected override void OnEnemyAwake()
        {
            _collider = GetComponent<MaggotCollider>();
            _collider.OnDamageTaken += ApplyDamage;
        }

        public override void Hit(int damage, int force)
        {
            ApplyDamage(damage + (force / 2));
            OnHit.Raise(this, damage, force);
        }

        protected override void OnDamage() { }

        protected override void OnDeath()
        {
            _collider.OnDamageTaken -= ApplyDamage;

            Instantiate(_deathExplosion.gameObject, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}