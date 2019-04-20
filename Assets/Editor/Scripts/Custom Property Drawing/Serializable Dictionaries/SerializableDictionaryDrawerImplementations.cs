using UnityEditor;
using UnityEngine;
using Utilities.UnitySerialization;

namespace CustomPropertyDrawing.SerializedDictionaries
{
    // ---------------
    //  Block Types => Texture2D
    // ---------------
    [CustomPropertyDrawer(typeof(BlockTypesTexture2DDictionary))]
    public class BlockTypesTexture2DPropertyDrawer : SerializableDictionaryDrawer<WorldObjects.Blocks.BlockTypes, Texture2D>
    {
        protected override SerializableKeyValueTemplate<WorldObjects.Blocks.BlockTypes, Texture2D> GetTemplate() => GetGenericTemplate<SerializableBlockTypesTexture2DTemplate>();
    }
    internal class SerializableBlockTypesTexture2DTemplate : SerializableKeyValueTemplate<WorldObjects.Blocks.BlockTypes, Texture2D> { }

    // ---------------
    //  Block Types => GameObjects
    // ---------------
    [CustomPropertyDrawer(typeof(BlockTypesGameObjectDictionary))]
    public class BlockTypesGameObjectsPropertyDrawer : SerializableDictionaryDrawer<WorldObjects.Blocks.BlockTypes, GameObject>
    {
        protected override SerializableKeyValueTemplate<WorldObjects.Blocks.BlockTypes, GameObject> GetTemplate() => GetGenericTemplate<BlockTypesGameObjectDictionaryTemplate>();
    }
    internal class BlockTypesGameObjectDictionaryTemplate : SerializableKeyValueTemplate<WorldObjects.Blocks.BlockTypes, GameObject> { }

    // ---------------
    //  Feature Types => GameObjects
    // ---------------
    [CustomPropertyDrawer(typeof(PropTypesGameObjectDictionary))]
    public class PropTypesGameObjectsPropertyDrawer : SerializableDictionaryDrawer<WorldObjects.WorldGeneration.PropGeneration.PropTypes, GameObject>
    {
        protected override SerializableKeyValueTemplate<WorldObjects.WorldGeneration.PropGeneration.PropTypes, GameObject> GetTemplate() => GetGenericTemplate<PropTypesGameObjectDictionaryTemplate>();
    }
    internal class PropTypesGameObjectDictionaryTemplate : SerializableKeyValueTemplate<WorldObjects.WorldGeneration.PropGeneration.PropTypes, GameObject> { }

    // ---------------
    //  Item Actor Types => GameObjects
    // ---------------
    [CustomPropertyDrawer(typeof(ItemActorTypesGameObjectDictionary))]
    public class ItemActorTypesGameObjectDictionaryDrawer : SerializableDictionaryDrawer<Items.ItemActors.ItemActorTypes, GameObject>
    {
        protected override SerializableKeyValueTemplate<Items.ItemActors.ItemActorTypes, GameObject> GetTemplate() => GetGenericTemplate<ItemActorTypesGameObjectDictionaryTemplate>();
    }
    internal class ItemActorTypesGameObjectDictionaryTemplate : SerializableKeyValueTemplate<Items.ItemActors.ItemActorTypes, GameObject> { }

    // ---------------
    //  Feature Types => GameObjects
    // ---------------
    [CustomPropertyDrawer(typeof(FeatureTypesGameObjectDictionary))]
    public class FeatureTypesGameObjectDictionaryDrawer : SerializableDictionaryDrawer<WorldObjects.WorldGeneration.FeatureGeneration.FeatureTypes, GameObject>
    {
        protected override SerializableKeyValueTemplate<WorldObjects.WorldGeneration.FeatureGeneration.FeatureTypes, GameObject> GetTemplate() => GetGenericTemplate<FeatureTypesGameObjectDictionaryTemplate>();
    }
    internal class FeatureTypesGameObjectDictionaryTemplate : SerializableKeyValueTemplate<WorldObjects.WorldGeneration.FeatureGeneration.FeatureTypes, GameObject> { }
}