using System;

using UnityEngine;

namespace Utilities.UnitySerialization
{
    // ---------------
    //  Block Types -> Texture2D
    // ---------------
    [Serializable]
    public class BlockTypesTexture2DDictionary : SerializableDictionary<WorldObjects.Blocks.BlockTypes, Texture2D> { }

    // ---------------
    //  Block Types -> GameObjects
    // ---------------
    [Serializable]
    public class BlockTypesGameObjectDictionary : SerializableDictionary<WorldObjects.Blocks.BlockTypes, GameObject> { }

    // ---------------
    //  Item Actor Types -> GameObjects
    // ---------------
    [Serializable]
    public class ItemActorTypesGameObjectDictionary : SerializableDictionary<Items.ItemActors.ItemActorTypes, GameObject> { }
}