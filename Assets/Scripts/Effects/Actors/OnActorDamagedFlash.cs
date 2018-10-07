using Actors;
using System.Collections;
using UnityEngine;

namespace Effects.Actors
{
    [RequireComponent(typeof(ActorData), typeof(SpriteRenderer))]
    public class OnActorDamagedFlash : MonoBehaviour
    {
#pragma warning disable IDE0044 // Add readonly modifier, Unity serialization requires it not be readonly
        [SerializeField]
        private int _numberFlashes = 0;
#pragma warning restore IDE0044 // Add readonly modifier

        private SpriteRenderer _sprite;
        private Coroutine _flashDamageCoroutine;

        private void Awake()
        {
            var actor = GetComponent<ActorData>();
            actor.OnActorDamaged += OnActorDamaged;

            _sprite = GetComponent<SpriteRenderer>();
        }

        private void OnActorDamaged(ActorData actor)
        {
            if(_flashDamageCoroutine == null)
            {
                _flashDamageCoroutine = StartCoroutine(FlashDamage(actor.DamageInvulnerabilityDuration));
            }
        }

        private IEnumerator FlashDamage(float flashDuration)
        {
            var waitFor = new WaitForSeconds(flashDuration / _numberFlashes / 2);

            for (var i = 0; i < _numberFlashes; i++)
            {
                _sprite.color = Color.red;
                yield return waitFor;
                _sprite.color = Color.white;
                yield return waitFor;
            }
        }
    }
}