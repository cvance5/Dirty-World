﻿using Data.Serialization.SerializableHazards;
using Data.Serialization.SerializableSpaces;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldObjects;
using WorldObjects.Hazards;
using WorldObjects.Spaces;

namespace Data.Serialization
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class SerializableChunk
    {
        public static SmartEvent<Chunk> OnChunkLoaded = new SmartEvent<Chunk>();

        [JsonProperty("position")]
        private readonly IntVector2 _position;

        [JsonProperty("topRight")]
        private readonly IntVector2 _topRight;

        [JsonProperty("botLeft")]
        private readonly IntVector2 _bottomLeft;

        [JsonProperty("blocks")]
        private readonly List<SerializableBlock> _blocks = new List<SerializableBlock>();

        [JsonProperty("spaces")]
        private readonly List<SerializableSpace> _spaces = new List<SerializableSpace>();

        [JsonProperty("hazards")]
        private readonly List<SerializableHazard> _hazards = new List<SerializableHazard>();

        [JsonProperty("enemies")]
        private readonly List<SerializableEnemy> _enemies = new List<SerializableEnemy>();

        [JsonProperty("items")]
        private readonly List<SerializableItem> _items = new List<SerializableItem>();

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

            foreach (var space in chunk.Spaces)
            {
                _spaces.Add(ToSerializableSpace(space));
            }

            foreach (var hazard in chunk.Hazards)
            {
                _hazards.Add(ToSerializableHazard(hazard));
            }

            foreach (var enemy in chunk.Enemies)
            {
                _enemies.Add(new SerializableEnemy(enemy));
            }

            foreach (var item in chunk.Items)
            {
                _items.Add(new SerializableItem(item));
            }
        }

        public string Serialize() => JsonConvert.SerializeObject(this, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });
        public static SerializableChunk Deserialize(string chunkJson) => JsonConvert.DeserializeObject<SerializableChunk>(chunkJson, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });

        public IEnumerator ToObject()
        {
            var chunk = new GameObject($"Chunk [{_position.X}, {_position.Y}]").AddComponent<Chunk>();
            chunk.transform.position = _position;
            chunk.AssignExtents(_bottomLeft, _topRight);

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

            foreach (var serializedSpace in _spaces)
            {
                var space = serializedSpace.ToObject();
                chunk.Register(space);

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

            OnChunkLoaded.Raise(chunk);
        }

        private SerializableSpace ToSerializableSpace(WorldObjects.Spaces.Space space)
        {
            if (space is Shaft)
            {
                return new SerializableShaft(space as Shaft);
            }
            else if (space is Corridor)
            {
                return new SerializableCorridor(space as Corridor);
            }
            else if (space is MonsterDen)
            {
                return new SerializableMonsterDen(space as MonsterDen);
            }
            else if(space is TreasureRoom)
            {
                return new SerializableTreasureRoom(space as TreasureRoom);
            }
            else if (space is Laboratory)
            {
                return new SerializableLaboratory(space as Laboratory);
            }
            else throw new System.Exception($"Unknown space type: {space.GetType().Name}. Cannot serialize.");
        }

        private SerializableHazard ToSerializableHazard(Hazard hazard)
        {
            if (hazard is StalagHazard)
            {
                return new SerializableStalag(hazard as StalagHazard);
            }
            else throw new System.Exception($"Unknown hazard type: {hazard.GetType().Name}.  Cannot serialize.");
        }
    }
}