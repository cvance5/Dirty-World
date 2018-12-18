using UnityEngine;
using Utilities.UnitySerialization;

namespace CustomPropertyDrawering.SerializedDictionaries
{
    // ---------------
    //  Block Types => Texture2D
    // ---------------
    [UnityEditor.CustomPropertyDrawer(typeof(BlockTypesTexture2DDictionary))]
    public class BlockTypesTexture2DPropertyDrawer : SerializableDictionaryDrawer<WorldObjects.Blocks.BlockTypes, Texture2D>
    {
        protected override SerializableKeyValueTemplate<WorldObjects.Blocks.BlockTypes, Texture2D> GetTemplate() => GetGenericTemplate<SerializableBlockTypesTexture2DTemplate>();
    }
    internal class SerializableBlockTypesTexture2DTemplate : SerializableKeyValueTemplate<WorldObjects.Blocks.BlockTypes, Texture2D> { }
}