﻿using Newtonsoft.Json;
using WorldObjects.Spaces;

namespace Data.Serialization.SerializableSpaces
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class SerializableShaft : SerializableSpace
    {
        [JsonProperty("isUncapped")]
        private readonly bool _isUncapped;

        [JsonProperty("bottomLeftCorner")]
        private readonly IntVector2 _bottomLeftCorner;
        [JsonProperty("topRightCorner")]
        private readonly IntVector2 _topRightCorner;

        [JsonConstructor]
        public SerializableShaft() { }

        public SerializableShaft(Shaft shaft)
        {
            _isUncapped = shaft.IsUncapped;
            _bottomLeftCorner = shaft.BottomLeftCorner;
            _topRightCorner = shaft.TopRightCorner;
        }

        public override Space ToObject() => new Shaft(_bottomLeftCorner, _topRightCorner, _isUncapped);
    }
}
