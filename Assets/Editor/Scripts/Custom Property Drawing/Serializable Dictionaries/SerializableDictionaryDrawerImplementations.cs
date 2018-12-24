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
    //  Item Actor Types => GameObjects
    // ---------------
    [CustomPropertyDrawer(typeof(ItemActorTypesGameObjectDictionary))]
    public class ItemActorTypesGameObjectDictionaryDrawer : SerializableDictionaryDrawer<Items.ItemActors.ItemActorTypes, GameObject>
    {
        protected override SerializableKeyValueTemplate<Items.ItemActors.ItemActorTypes, GameObject> GetTemplate() => GetGenericTemplate<ItemActorTypesGameObjectDictionaryTemplate>();
    }
    internal class ItemActorTypesGameObjectDictionaryTemplate : SerializableKeyValueTemplate<Items.ItemActors.ItemActorTypes, GameObject> { }
}