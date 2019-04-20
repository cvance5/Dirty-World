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
    //  Prop Types -> GameObjects
    // ---------------
    [Serializable]
    public class PropTypesGameObjectDictionary : SerializableDictionary<WorldObjects.WorldGeneration.PropGeneration.PropTypes, GameObject> { }

    // ---------------
    //  Item Actor Types -> GameObjects
    // ---------------
    [Serializable]
    public class ItemActorTypesGameObjectDictionary : SerializableDictionary<Items.ItemActors.ItemActorTypes, GameObject> { }

    // ---------------
    //  Feature Types -> GameObjects
    // ---------------
    [Serializable]
    public class FeatureTypesGameObjectDictionary : SerializableDictionary<WorldObjects.WorldGeneration.FeatureGeneration.FeatureTypes, GameObject> { }
}