using MathConcepts;
using System.Collections.Generic;
using UnityEngine;

namespace Effects.Audio
{
    public class OnHitSoundCue : MonoBehaviour
    {
#pragma warning disable IDE0044 // Add readonly modifier, Unity serialization requires it not be readonly
        [SerializeField]
        [Tooltip("One sound is randomly selected per hit, if the damage is strictly greater than the force.")]
        private List<AudioClip> _onDamageSounds = null;

        [SerializeField]
        [Tooltip("One sound is randomly selected per hit, if the force is greater than or equal to the damage.")]
        private List<AudioClip> _onForceSounds = null;

        [SerializeField]
        [Tooltip("Scales volume as damage and force increase in this range.")]
        private Range _volumeScale = new Range(0, 10);
#pragma warning restore IDE0044 // Add readonly modifier

        private AudioClip _lastPlayed;

        private void Awake()
        {
            if (!(GetComponent(typeof(IHittable)) is IHittable attachedHittable)) throw new MissingComponentException($"OnHitSoundCue on {gameObject.name} has no IHittable to cue off of.");
            else attachedHittable.OnHit += OnHit;
        }

        private void OnHit(IHittable hitObject, int damage, int force)
        {
            AudioClip soundToPlay;
            int hitIntensity;

            if (damage > force)
            {
                soundToPlay = _onDamageSounds.RandomItem(_lastPlayed);
                hitIntensity = damage;
            }
            else
            {
                soundToPlay = _onForceSounds.RandomItem(_lastPlayed);
                hitIntensity = force;
            }

            var scaledVolume = MathUtils.MapRange(hitIntensity, _volumeScale.Min, _volumeScale.Max, 0f, 1f);

            if (scaledVolume > 0)
            {
                AudioSource.PlayClipAtPoint(soundToPlay, transform.position, scaledVolume);
                _lastPlayed = soundToPlay;
            }
        }
    }
}