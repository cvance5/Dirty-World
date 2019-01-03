using Data;
using Data.IO;
using Data.Serialization;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using Utilities.Debug;
using WorldObjects.Blocks;
using WorldObjects.WorldGeneration.BlockGeneration;

namespace WorldObjects.WorldGeneration
{
    public class WorldBuilder
    {
        private int _chunkSize => _world.ChunkSize;

        private readonly World _world;
        private readonly SpacePicker _sPicker;
        private readonly BlockPicker _bPicker;

        private IntVector2 _chunkBeingActivated;
        private readonly Queue<IntVector2> _chunksToActivate = new Queue<IntVector2>();

        private Coroutine _chunkActivationCoroutine;

        private readonly Dictionary<BlockTypes, Range> _fillRanges;

        public WorldBuilder(World world, SpacePicker sPicker = null, BlockPicker bPicker = null)
        {
            _world = world;
            _sPicker = sPicker;
            _bPicker = bPicker;
            _fillRanges = new Dictionary<BlockTypes, Range>()
            {
                {BlockTypes.None, new Range(_world.SurfaceDepth + 1, int.MaxValue) },
                {BlockTypes.Dirt, new Range(int.MinValue, _world.SurfaceDepth) }
            };

            ChunkBuilder.OnChunkBuilt += OnChunkReadyToActivate;
            SerializableChunk.OnChunkLoaded += OnChunkReadyToActivate;
        }

        public void ActivateChunk(IntVector2 worldPosition)
        {
            if (_world.GetChunkAtPosition(worldPosition) != null) return;
            else if (_chunkBeingActivated == worldPosition) return;
            else if (_chunksToActivate.Contains(worldPosition)) return;
            else if (_chunkBeingActivated == null)
            {
                if (GameSaves.HasGameData(worldPosition.ToString()))
                {
                    _log.Info($"Loading Chunk at {worldPosition}.");
                    LoadChunk(worldPosition);
                }
                else
                {
                    _log.Info($"Building Chunk at {worldPosition}.");
                    BuildChunk(worldPosition);
                }
            }
            else _chunksToActivate.Enqueue(worldPosition);
        }

        private void LoadChunk(IntVector2 worldPosition)
        {
            _chunkBeingActivated = worldPosition;

            var serializedChunk = DataReader.Read(worldPosition.ToString(), DataTypes.CurrentGame);
            var serializableChunk = SerializableChunk.Deserialize(serializedChunk);

            // Calls OnChunkReadyToActivate when finished
            _chunkActivationCoroutine = CoroutineHandler.StartCoroutine(serializableChunk.ToObject);
        }

        private void BuildChunk(IntVector2 worldPosition)
        {
            _chunkBeingActivated = worldPosition;

            var cBuilder = new ChunkBuilder(worldPosition, _chunkSize, _world.GetBlueprintForPosition(worldPosition));

            if (_sPicker != null)
            {
                var sBuilders = _sPicker.Select(cBuilder);
                foreach (var sBuilder in sBuilders)
                {
                    cBuilder.AddSpace(sBuilder);
                }
            }

            if (_bPicker != null)
            {
                var blocks = _bPicker.Pick(cBuilder);

                cBuilder.AddBlocks(blocks);
            }

            cBuilder.SetFill(GetFill(cBuilder.Depth));

            // Calls OnChunkReadyToActivate when finished
            _chunkActivationCoroutine = CoroutineHandler.StartCoroutine(cBuilder.Build);
        }

        private void OnChunkReadyToActivate(Chunk chunk)
        {
            _world.Register(chunk);
            _chunkBeingActivated = null;
            _chunkActivationCoroutine = null;

            if (_chunksToActivate.Count > 0)
            {
                ActivateChunk(_chunksToActivate.Dequeue());
            }
        }

        private BlockTypes GetFill(int depth)
        {
            foreach (var fillRange in _fillRanges)
            {
                if (fillRange.Value.IsInRange(depth)) return fillRange.Key;
            }

            throw new System.ArgumentOutOfRangeException($"Unhandled depth {depth}.  No fill block found.");
        }

        public void Destroy()
        {
            _chunksToActivate.Clear();
            _chunkBeingActivated = null;
            if (_chunkActivationCoroutine != null)
            {
                CoroutineHandler.AbortCoroutine(_chunkActivationCoroutine);
            }

            ChunkBuilder.OnChunkBuilt -= OnChunkReadyToActivate;
            SerializableChunk.OnChunkLoaded -= OnChunkReadyToActivate;
        }

        private static readonly Log _log = new Log("WorldBuilder");
    }
}
