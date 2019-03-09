using System;
using System.Collections;

namespace WorldObjects.WorldGeneration.ChunkGeneration
{
    public class ChunkActivationCommand
    {
        public readonly Chunk Chunk;
        private readonly Func<Chunk, IEnumerator> _command;

        public ChunkActivationCommand(Chunk chunk, Func<Chunk, IEnumerator> command)
        {
            Chunk = chunk;
            _command = command;
        }

        public IEnumerator Invoke() => _command.Invoke(Chunk);
    }
}