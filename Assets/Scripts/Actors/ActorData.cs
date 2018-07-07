using System.Collections;
using UnityEngine;

namespace Actors
{
    public abstract class ActorData : MonoBehaviour, ITrackable
    {
        public int Health { get; private set; } = 100;
        public IntVector2 GetPosition() => new IntVector2(transform.position);

        [SerializeField]
        private float _damageInvulnerabilityDuration = 1f;
        [SerializeField]
        private int _damageResistance = 5;

        private SpriteRenderer _sprite;
        private bool _isTakingDamage = false;

        private void Awake()
        {
            _sprite = GetComponent<SpriteRenderer>();
        }

        public void ApplyDamage(int amount)
        {
            if (amount < _damageResistance) return;
            else amount -= _damageResistance;

            if (!_isTakingDamage)
            {
                Health -= amount;

                if (Health <= 0) Die();
                else StartCoroutine(FlashDamage());
            }
        }

        private IEnumerator FlashDamage()
        {
            _isTakingDamage = true;
            var waitFor = new WaitForSeconds(_damageInvulnerabilityDuration / NUM_FLASHES);

            for (int i = 0; i < NUM_FLASHES; i++)
            {
                _sprite.color = Color.red;
                yield return waitFor;
                _sprite.color = Color.white;
                yield return waitFor;
            }
            _isTakingDamage = false;
        }

        protected abstract void Die();

        private const int NUM_FLASHES = 4;
    }
}