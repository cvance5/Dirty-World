using Data;
using WorldObjects.WorldGeneration;

namespace WorldObjects
{
    public class ChunkActivationController
    {
        private readonly World _world;
        private readonly WorldBuilder _worldBuilder;

        public ChunkActivationController(World world, WorldBuilder worldBuilder)
        {
            _world = world;
            _worldBuilder = worldBuilder;

            CheckForGenerateChunk(new IntVector2(0, 0), 0);
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
                CheckForGenerateChunk(newPosition.Chunk.Position, 0);
            }
        }

        private void CheckForGenerateChunk(IntVector2 currentChunkPosition, int depth)
        {
            foreach (var dir in Directions.Compass)
            {
                var newChunkPosition = _world.GetChunkPosition(new IntVector2(currentChunkPosition), dir);
                _worldBuilder.ActivateChunk(newChunkPosition);

                if (depth < ACTIVATION_DEPTH)
                {
                    CheckForGenerateChunk(newChunkPosition, depth + 1);
                }
            }
        }

        private const int ACTIVATION_DEPTH = 2;
    }
}