using System.Collections.Generic;
using UnityEngine;
using WorldObjects;

namespace Effects.Audio
{
    public class OnWorldObjectDestroyedSoundCue : MonoBehaviour
    {
#pragma warning disable IDE0044 // Add readonly modifier, Unity serialization requires it not be readonly
        [SerializeField]
        [Tooltip("One sound is randomly selected on destruction.")]
        private List<AudioClip> _onWorldObjectDestroyedSounds = null;
#pragma warning restore IDE0044 // Add readonly modifier

        private void Awake()
        {
            var attachedWorldObject = GetComponent<WorldObject>();
            if (attachedWorldObject == null) throw new MissingComponentException($"OnWorldObjectDestroyedSoundCue on {gameObject.name} has no WorldObject to cue off of.");
            else attachedWorldObject.OnWorldObjectDestroyed += OnWorldObjectDestroyed;
        }

        private void OnWorldObjectDestroyed(WorldObject worldObject)
        {
            AudioSource.PlayClipAtPoint(_onWorldObjectDestroyedSounds.RandomItem(), transform.position);
        }
    }
}