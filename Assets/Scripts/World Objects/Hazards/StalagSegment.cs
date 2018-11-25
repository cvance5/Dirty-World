using UnityEngine;

namespace WorldObjects.Hazards
{
    public class StalagSegment : WorldObject, IHittable
    {
        public SmartEvent<IHittable, int, int> OnHit { get; set; } = new SmartEvent<IHittable, int, int>();

        public int Health { get; set; }
        public int Stability { get; set; }

        private bool _isPartOfStalag;

        public override string ObjectName => gameObject.name;

        private Rigidbody2D _rigibody;

        protected override void OnWorldObjectAwake() => _rigibody = GetComponent<Rigidbody2D>();

        public void Initialize(int health, int stability)
        {
            Health = health;
            Stability = stability;
            _isPartOfStalag = true;
        }

        public void Crumble()
        {
            _rigibody.bodyType = RigidbodyType2D.Dynamic;
            _isPartOfStalag = false;
        }

        public void Hit(int damage, int force)
        {
            if (_isPartOfStalag)
            {
                OnHit.Raise(this, damage, force);
            }
            else
            {
                Health -= damage + (force / 2);

                if (Health <= 0)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}