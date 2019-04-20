using Data.Serialization.SerializableFeatures;
using Data.Serialization.SerializableHazards;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldObjects;

namespace Data.Serialization
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class SerializableChunk : ISerializable<Chunk>
    {
        [JsonProperty("position")]
        private readonly IntVector2 _position;

        [JsonProperty("topRight")]
        private readonly IntVector2 _topRight;

        [JsonProperty("botLeft")]
        private readonly IntVector2 _bottomLeft;

        [JsonProperty("blocks")]
        private readonly List<SerializableBlock> _blocks = new List<SerializableBlock>();

        [JsonProperty("props")]
        private readonly List<SerializableProp> _props = new List<SerializableProp>();

        [JsonProperty("features")]
        private readonly List<SerializableFeature> _features = new List<SerializableFeature>();

        [JsonProperty("hazards")]
        private readonly List<SerializableHazard> _hazards = new List<SerializableHazard>();

        [JsonProperty("enemies")]
        private readonly List<SerializableEnemy> _enemies = new List<SerializableEnemy>();

        [JsonProperty("items")]
        private readonly List<SerializableItem> _items = new List<SerializableItem>();

        [JsonProperty("spacesUsed")]
        private readonly List<string> _spacesUsed = new List<string>();

        [JsonConstructor]
        public SerializableChunk() { }

        public SerializableChunk(Chunk chunk)
        {
            _position = chunk.Position;
            _topRight = chunk.TopRightCorner;
            _bottomLeft = chunk.BottomLeftCorner;

            foreach (var kvp in chunk.BlockMap)
            {
                _blocks.Add(new SerializableBlock(kvp.Value));
            }

            foreach (var kvp in chunk.PropMap)
            {
                _props.Add(new SerializableProp(kvp.Value));
            }

            foreach (var feature in chunk.Features)
            {
                _features.Add(SerializableFeatureHelper.ToSerializableFeature(feature));
            }

            foreach (var hazard in chunk.Hazards)
            {
                _hazards.Add(SerializableHazardHelper.ToSerializableHazard(hazard));
            }

            foreach (var enemy in chunk.Enemies)
            {
                _enemies.Add(new SerializableEnemy(enemy));
            }

            foreach (var item in chunk.Items)
            {
                _items.Add(new SerializableItem(item));
            }

            _spacesUsed = chunk.SpacesUsed;
        }

        public string Serialize() => JsonConvert.SerializeObject(this, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });
        public static SerializableChunk Deserialize(string chunkJson) => JsonConvert.DeserializeObject<SerializableChunk>(chunkJson, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });

        public Chunk ToObject()
        {
            var chunk = new GameObject($"Chunk [{_position.X}, {_position.Y}]").AddComponent<Chunk>();
            chunk.transform.position = _position;
            chunk.AssignExtents(_bottomLeft, _topRight);
            chunk.SetState(Chunk.ChunkState.Constructing);

            return chunk;
        }

        public IEnumerator ReconstructCoroutine(Chunk chunk)
        {
            // Never use more than 1/3 of a frame
            var yieldTimer = new IncrementalTimer(Time.realtimeSinceStartup, 1f / 180f);

            foreach (var serializableBlock in _blocks)
            {
                var block = serializableBlock.ToObject();
                chunk.Register(block);

                if (yieldTimer.CheckIncrement(Time.realtimeSinceStartup))
                {
                    yield return null;
                    yieldTimer.AdvanceIncrement(Time.realtimeSinceStartup);
                }
            }

            foreach (var serializableProp in _props)
            {
                var prop = serializableProp.ToObject();
                chunk.Register(prop);

                if (yieldTimer.CheckIncrement(Time.realtimeSinceStartup))
                {
                    yield return null;
                    yieldTimer.AdvanceIncrement(Time.realtimeSinceStartup);
                }
            }

            foreach (var serializableFeature in _features)
            {
                var feature = serializableFeature.ToObject();
                chunk.Register(feature);

                if (yieldTimer.CheckIncrement(Time.realtimeSinceStartup))
                {
                    yield return null;
                    yieldTimer.AdvanceIncrement(Time.realtimeSinceStartup);
                }
            }

            foreach (var serializedHazard in _hazards)
            {
                var hazard = serializedHazard.ToObject();
                chunk.Register(hazard);

                if (yieldTimer.CheckIncrement(Time.realtimeSinceStartup))
                {
                    yield return null;
                    yieldTimer.AdvanceIncrement(Time.realtimeSinceStartup);
                }
            }

            foreach (var serializedEnemy in _enemies)
            {
                var enemy = serializedEnemy.ToObject();
                chunk.Register(enemy);

                if (yieldTimer.CheckIncrement(Time.realtimeSinceStartup))
                {
                    yield return null;
                    yieldTimer.AdvanceIncrement(Time.realtimeSinceStartup);
                }
            }

            foreach (var serializedItem in _items)
            {
                var item = serializedItem.ToObject();
                chunk.Register(item);

                if (yieldTimer.CheckIncrement(Time.realtimeSinceStartup))
                {
                    yield return null;
                    yieldTimer.AdvanceIncrement(Time.realtimeSinceStartup);
                }
            }

            foreach (var spaceUsed in _spacesUsed)
            {
                chunk.Register(spaceUsed);
            }

            chunk.SetState(Chunk.ChunkState.Ready);
        }
    }
}