using System;
using UnityEngine;

namespace Narrative
{
    [Serializable]
    public class ScriptKey
    {
        [SerializeField]
        private string _key;
        public string Key => _key.ToLowerInvariant().Trim();

        public ScriptKey(string key)
        {
            if (key == null)
            {
                key = string.Empty;
            }

            _key = key;
        }

        public static bool operator ==(ScriptKey lhs, ScriptKey rhs) => lhs.Key == rhs.Key;
        public static bool operator !=(ScriptKey lhs, ScriptKey rhs) => lhs.Key != rhs.Key;

        public static implicit operator ScriptKey(string rhs) => new ScriptKey(rhs);

        public override bool Equals(object obj)
        {
            if (obj is ScriptKey)
            {
                var scriptKeyObj = obj as ScriptKey;
                return Key.Equals(scriptKeyObj.Key);
            }
            else return false;
        }

        public override int GetHashCode() => Key.GetHashCode();
    }
}