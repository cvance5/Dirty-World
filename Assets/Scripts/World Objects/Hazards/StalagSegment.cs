using UnityEngine;

namespace WorldObjects.Hazards
{
    public class StalagSegment : Hazard, IHittable
    {
        public SmartEvent<IHittable, int, int> OnHit { get; set; } = new SmartEvent<IHittable, int, int>();

        public int Health { get; set; }
        public int Stability { get; set; }

#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly if we want Unity to serialize it
        [SerializeField]
        private int _baseImpactDamage = 10;

        [SerializeField]
        private float _weight = 1.75f;

        [SerializeField]
        private int _impactAbsorption = 5;
#pragma warning restore IDE0044 // Add readonly modifier

        private bool _isPartOfStalag;

        public override string ObjectName => gameObject.name;

        public override HazardEffects[] Effects => new HazardEffects[] { };
        public override HazardTypes Type => HazardTypes.Stalag;

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

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!_isPartOfStalag)
            {
                var other = collision.collider.gameObject;

                var impactMagnitude = Mathf.RoundToInt(collision.relativeVelocity.magnitude);
                var unabsorbedImpactMagnitude = impactMagnitude - _impactAbsorption;

                if (unabsorbedImpactMagnitude > 0)
                {
                    unabsorbedImpactMagnitude += _baseImpactDamage;

                    var otherHittable = other.GetComponent<IHittable>();
                    if(otherHittable != null)
                    {
                        otherHittable.Hit(unabsorbedImpactMagnitude, unabsorbedImpactMagnitude);
                    }                   

                    var unabsorbedImpact = collision.relativeVelocity.normalized * unabsorbedImpactMagnitude;

                    var otherImpulsable = other.GetComponent<IImpulsable>();
                    if (otherImpulsable != null)
                    {
                        otherImpulsable.Impulse(unabsorbedImpact * _weight);
                    }

                    Hit(unabsorbedImpactMagnitude, unabsorbedImpactMagnitude);
                }
            }
        }
    }
}