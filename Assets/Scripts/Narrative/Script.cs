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
        private TextAsset _source = null;
#pragma warning restore IDE0044 // Add readonly modifier

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

        private const string NO_KEY_FOUND = "No key found.";

        private static readonly Log _log = new Log("Script");
    }
}