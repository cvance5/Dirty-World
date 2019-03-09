using Data.Serialization.SerializableSpaces;
using System;
using UnityEngine;

namespace WorldObjects.Spaces
{
    [Serializable]
    public class CustomSpace : ScriptableObject
    {
        private string _serializedSpaceJson;

        public void Set(SerializableSpace space) => _serializedSpaceJson = space.Serialize();
        public Space Build() => SerializableSpace.Deserialize(_serializedSpaceJson).ToObject();
    }
}