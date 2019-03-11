using Data.Serialization.SerializableSpaces;
using UnityEngine;

namespace WorldObjects.Spaces
{
    public class CustomSpace : ScriptableObject
    {
        [SerializeField]
        private string _serializedSpaceJson;

        public void Set(SerializableSpace space) => _serializedSpaceJson = space.Serialize();
        public Space Build() => SerializableSpace.Deserialize(_serializedSpaceJson).ToObject();
    }
}