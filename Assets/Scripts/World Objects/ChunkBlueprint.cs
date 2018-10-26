using System.Collections.Generic;
using UnityEngine;
using WorldObjects.Actors.Enemies;
using Space = WorldObjects.Spaces.Space;

namespace WorldObjects
{
    public class ChunkBlueprint : IBoundary, ITrackable
    {
        public static SmartEvent<ChunkBlueprint> OnBlueprintChanged = new SmartEvent<ChunkBlueprint>();

        public IntVector2 Position { get; private set; }
        public IntVector2 BottomLeftCorner { get; protected set; }
        public IntVector2 TopRightCorner { get; protected set; }

        public List<Space> Spaces { get; private set; } = new List<Space>();
        public List<EnemyData> Enemies { get; private set; } = new List<EnemyData>();

        private static int _chunkSize = GameManager.Instance.Settings.ChunkSize;

        public ChunkBlueprint(IntVector2 position)
        {
            Position = position;

            BottomLeftCorner = new IntVector2(Position.X - (_chunkSize / 2), Position.Y - (_chunkSize / 2));
            TopRightCorner = new IntVector2(Position.X + (_chunkSize / 2), Position.Y + (_chunkSize / 2));
        }

        public bool Contains(IntVector2 position) =>
            position.X >= BottomLeftCorner.X &&
            position.Y >= BottomLeftCorner.Y &&
            position.X <= TopRightCorner.X &&
            position.Y <= TopRightCorner.Y;

        public void Register(Space space)
        {
            Spaces.Add(space);

            var edgesReached = new List<IntVector2>();

            foreach (var extentPoint in space.Extents)
            {
                if (!Contains(extentPoint))
                {
                    edgesReached = GetEdgesReached(extentPoint);
                }

                // We are already overlapping all edges with 
                // the previous extents, so don't check the rest.
                if (edgesReached.Count == 4) break;
            }

            OnBlueprintChanged.Raise(this);
        }

        public void Register(EnemyData enemy)
        {
            Enemies.Add(enemy);
            enemy.SetActive(false);

            _log.Info($"Hiding enemy {enemy.ObjectName}.");

            OnBlueprintChanged.Raise(this);
        }

        protected List<IntVector2> GetEdgesReached(IntVector2 extentPoint)
        {
            var edgesReached = new List<IntVector2>();

            if (extentPoint.X < BottomLeftCorner.X) edgesReached.Add(Vector2.left);
            if (extentPoint.Y < BottomLeftCorner.Y) edgesReached.Add(Vector2.down);
            if (extentPoint.X > BottomLeftCorner.X) edgesReached.Add(Vector2.right);
            if (extentPoint.Y > BottomLeftCorner.Y) edgesReached.Add(Vector2.up);

            return edgesReached;
        }

        private static readonly Log _log = new Log("Chunk Blueprint");
    }
}