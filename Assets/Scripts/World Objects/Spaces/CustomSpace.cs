using Data.Serialization.SerializableSpaces;
using UnityEngine;

public class CustomSpace : ScriptableObject
{
    private SerializableSpace _space;

    public void Set(SerializableSpace space) => _space = space;
    public WorldObjects.Spaces.Space Load() => _space.ToObject();
}