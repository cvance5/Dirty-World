using Data.Serialization.SerializableSpaces;
using UnityEngine;

namespace WorldObjects.Spaces
{
    public class CustomSpace : ScriptableObject
    {
        public static SmartEvent<Space> OnCustomSpaceBuilt = new SmartEvent<Space>();

        [SerializeField]
        private string _serializedSpaceJson;

        public void Set(SerializableSpace space) => _serializedSpaceJson = space.Serialize();

        public Space Build()
        {
            var space = SerializableSpace.Deserialize(_serializedSpaceJson).ToObject();

            OnCustomSpaceBuilt.Raise(space);

            return space;
        }
    }
}