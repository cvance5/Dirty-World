﻿using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Utilities.UnitySerialization
{
    abstract public class SerializableDictionary<K, V> : ISerializationCallbackReceiver, IDictionary<K, V>
    {
        [SerializeField]
        private K[] keys;
        [SerializeField]
        private V[] values;

        public Dictionary<K, V> dictionary = new Dictionary<K, V>();

        public ICollection<K> Keys => ((IDictionary<K, V>)dictionary).Keys;
        public ICollection<V> Values => ((IDictionary<K, V>)dictionary).Values;
        public int Count => ((IDictionary<K, V>)dictionary).Count;
        public bool IsReadOnly => ((IDictionary<K, V>)dictionary).IsReadOnly;
        public V this[K key] { get => ((IDictionary<K, V>)dictionary)[key]; set => ((IDictionary<K, V>)dictionary)[key] = value; }

        public static T New<T>() where T : SerializableDictionary<K, V>, new()
        {
            var result = new T
            {
                dictionary = new Dictionary<K, V>()
            };
            return result;
        }

        public void OnAfterDeserialize()
        {
            var c = keys.Length;
            dictionary = new Dictionary<K, V>(c);
            for (var i = 0; i < c; i++)
            {
                dictionary[keys[i]] = values[i];
            }
            keys = null;
            values = null;
        }

        public void OnBeforeSerialize()
        {
            var c = dictionary.Count;
            keys = new K[c];
            values = new V[c];
            var i = 0;
            using (var e = dictionary.GetEnumerator())
                while (e.MoveNext())
                {
                    var kvp = e.Current;
                    keys[i] = kvp.Key;
                    values[i] = kvp.Value;
                    i++;
                }
        }

        public void Add(K key, V value) => ((IDictionary<K, V>)dictionary).Add(key, value);
        public bool ContainsKey(K key) => ((IDictionary<K, V>)dictionary).ContainsKey(key);
        public bool Remove(K key) => ((IDictionary<K, V>)dictionary).Remove(key);
        public bool TryGetValue(K key, out V value) => ((IDictionary<K, V>)dictionary).TryGetValue(key, out value);
        public void Add(KeyValuePair<K, V> item) => ((IDictionary<K, V>)dictionary).Add(item);
        public void Clear() => ((IDictionary<K, V>)dictionary).Clear();
        public bool Contains(KeyValuePair<K, V> item) => ((IDictionary<K, V>)dictionary).Contains(item);
        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex) => ((IDictionary<K, V>)dictionary).CopyTo(array, arrayIndex);
        public bool Remove(KeyValuePair<K, V> item) => ((IDictionary<K, V>)dictionary).Remove(item);
        public IEnumerator<KeyValuePair<K, V>> GetEnumerator() => ((IDictionary<K, V>)dictionary).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IDictionary<K, V>)dictionary).GetEnumerator();
    }
}