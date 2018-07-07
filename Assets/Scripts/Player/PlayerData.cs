using System.Collections;
using UnityEngine;

namespace Player
{
    public class PlayerData : MonoBehaviour, ITrackable
    {
        public int Health { get; private set; } = 100;

        public IntVector2 GetPosition() => new IntVector2(transform.position);
        public uint GoldCollected { get; private set; }

        private SpriteRenderer _sprite;
        private bool _isTakingDamage = false;

        private void Awake()
        {
            _sprite = GetComponent<SpriteRenderer>();
        }

        public void ApplyDamage(int amount)
        {
            if (!_isTakingDamage)
            {
                Health -= amount;

                if (Health <= 0) Die();
                else StartCoroutine(FlashDamage());
            }
        }

        public void AddItem(ItemTypes item)
        {
            if (item == ItemTypes.GoldPiece) GoldCollected++;
        }

        private IEnumerator FlashDamage()
        {
            _isTakingDamage = true;
            var waitFor = new WaitForSeconds(.25f);

            for (int i = 0; i < 4; i++)
            {
                _sprite.color = Color.red;
                yield return waitFor;
                _sprite.color = Color.white;
                yield return waitFor;
            }
            _isTakingDamage = false;
        }

        private void Die()
        {
            PositionTracker.StopTracking(this);
            Log.Info($"Score: {GoldCollected}", "blue");

            Destroy(gameObject);
        }
    }
}