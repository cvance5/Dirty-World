using Data;
using System.Collections.Generic;
using System.Linq;

namespace WorldObjects
{
    public class ChunkActivationController
    {
        private int _activationDepth = 2;
        private readonly World _world;

        private IntVector2 _position = new IntVector2(0, 0);

        public ChunkActivationController(World world, int activationDepth)
        {
            _world = world;
            ChangeActivationDepth(activationDepth);
        }

        public void ChangeActivationDepth(int newActivationDepth)
        {
            _activationDepth = newActivationDepth;
            var activeChunkList = BuildActiveChunkList(_position);
            _world.SetActiveChunks(activeChunkList);
        }

        public void ListenTo(ITrackable activationCatalyst)
        {
            PositionTracker.Subscribe(activationCatalyst, OnPositionUpdate);
            var position = PositionTracker.GetCurrentPosition(activationCatalyst);

            OnPositionUpdate(activationCatalyst, null, position);
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
                var activeChunkList = BuildActiveChunkList(newPosition.Chunk.Position);

                _world.SetActiveChunks(activeChunkList);
                _position = newPosition.Chunk.Position;
            }
        }

        private List<IntVector2> BuildActiveChunkList(IntVector2 currentChunkPosition, int depth = 0)
        {
            var activeChunks = new List<IntVector2>()
            {
                currentChunkPosition
            };

            foreach (var dir in Directions.Compass)
            {
                var newChunkPosition = _world.GetChunkPosition(new IntVector2(currentChunkPosition), dir);

                if (depth < _activationDepth - 1)
                {
                    activeChunks.AddRange(BuildActiveChunkList(newChunkPosition, depth + 1));
                }
                activeChunks.Add(newChunkPosition);
            }

            return activeChunks.Distinct().ToList();
        }
    }
}