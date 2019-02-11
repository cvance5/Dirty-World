using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Debug;

namespace Narrative
{
    public class Script : ScriptableObject
    {
#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly if we want Unity Serialization
        [SerializeField]
        private bool _skipScriptPlayback = false;

        [SerializeField]
        private TextAsset _source = null;
#pragma warning restore IDE0044 // Add readonly modifier

        private static readonly List<IScriptedPlaybackListener> _scriptListeners = new List<IScriptedPlaybackListener>();

        private static int _playbackSpeed = 1;

        private static Dictionary<ScriptKey, string> _scriptMap;

        private void OnEnable()
        {
            if (_source != null)
            {
                if (_scriptMap != null)
                {
                    _log.Error($"Multiple scripts active at a time.  Clearing.");
                }

                _scriptMap = JsonConvert.DeserializeObject<Dictionary<ScriptKey, string>>(_source.text);
            }

#if UNITY_EDITOR
            if (_skipScriptPlayback)
            {
                _playbackSpeed = 1000;
                UpdatePlaybackSpeed();
            }
#endif  
        }

        public static string Read(ScriptKey key)
        {
            if (!_scriptMap.TryGetValue(key, out var value))
            {
                _log.Warning($"Cannot find line for key {key}.");
                value = NO_KEY_FOUND;
            }

            return value;
        }

        public static void Subscribe(IScriptedPlaybackListener listener)
        {
            if (_scriptListeners.Contains(listener))
            {
                throw new System.InvalidOperationException($"{listener} is already subscribed to this script.");
            }
            else _scriptListeners.Add(listener);

            listener.SetPlaybackSpeed(_playbackSpeed);
        }

        public static void Unsubscribe(IScriptedPlaybackListener listener)
        {
            if (!_scriptListeners.Remove(listener))
            {
                throw new System.InvalidOperationException($"{listener} is not subscribed to this script and can't be removed.");
            }
        }

        private static void UpdatePlaybackSpeed()
        {
            foreach (var listener in _scriptListeners)
            {
                listener.SetPlaybackSpeed(_playbackSpeed);
            }
        }

        private const string NO_KEY_FOUND = "No key found.";

        private static readonly Log _log = new Log("Script");
    }
}