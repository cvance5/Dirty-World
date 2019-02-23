using Data.Serialization.SerializableSpaces;
using UnityEngine;

namespace WorldObjects.Spaces
{
    public class CustomSpace : ScriptableObject
    {
        private string _serializedSpaceJson;

        public void Set(SerializableSpace space) => _serializedSpaceJson = space.Serialize();
        public Space Build() => SerializableSpace.Deserialize(_serializedSpaceJson).ToObject();
    }
}