using System;

using UnityEngine;

namespace Utilities.UnitySerialization
{
    // ---------------
    //  String => Int
    // ---------------
    [Serializable]
    public class BlockTypesTexture2DDictionary : SerializableDictionary<WorldObjects.Blocks.BlockTypes, Texture2D> { }
}