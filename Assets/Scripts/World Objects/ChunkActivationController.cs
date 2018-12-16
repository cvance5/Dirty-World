using Data;
using System.Collections.Generic;

namespace WorldObjects
{
    public class ChunkActivationController
    {
        private readonly World _world;

        public ChunkActivationController(World world)
        {
            _world = world;

            BuildActiveChunkList(new IntVector2(0, 0), 0);
        }

        public void ListenTo(ITrackable activationCatalyst)
        {
            PositionTracker.Subscribe(activationCatalyst, OnPositionUpdate);
            var currentPosition = PositionTracker.GetCurrentPosition(activationCatalyst);

            OnPositionUpdate(activationCatalyst, null, currentPosition);
        }

        private void OnPositionUpdate(ITrackable trackable, PositionData oldPosition, PositionData newPosition)
        {
            if (newPosition.Chunk == null)
            {
                Timekeeper.TogglePause(true);
            }
            else
            {
                Timekeeper.TogglePause(false);
                var activeChunkList = BuildActiveChunkList(newPosition.Chunk.Position, 0);

                _world.SetActiveChunks(activeChunkList);
            }
        }

        private List<IntVector2> BuildActiveChunkList(IntVector2 currentChunkPosition, int depth)
        {
            var activeChunks = new List<IntVector2>();

            foreach (var dir in Directions.Compass)
            {
                var newChunkPosition = _world.GetChunkPosition(new IntVector2(currentChunkPosition), dir);

                if (depth < ACTIVATION_DEPTH)
                {
                    activeChunks.AddRange(BuildActiveChunkList(newChunkPosition, depth + 1));
                }
                activeChunks.Add(newChunkPosition);
            }

            return activeChunks;
        }

        private const int ACTIVATION_DEPTH = 2;
    }
}