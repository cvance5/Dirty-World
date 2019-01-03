using System;
using System.Collections.Generic;
using System.Linq;
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

        private static World _world;

        public static void AssignWorld(World world) => _world = world;

        public ChunkBlueprint(IntVector2 position, IntVector2 bottomLeftCorner, IntVector2 topRightCorner)
        {
            Position = position;

            BottomLeftCorner = bottomLeftCorner;
            TopRightCorner = topRightCorner;
        }

        public bool Contains(IntVector2 position) =>
            position.X >= BottomLeftCorner.X &&
            position.Y >= BottomLeftCorner.Y &&
            position.X <= TopRightCorner.X &&
            position.Y <= TopRightCorner.Y;

        public void Register(Space space)
        {
            if (Spaces.Contains(space)) return;

            Spaces.Add(space);

            var edgesReached = new List<IntVector2>();

            foreach (var extentPoint in space.Extents)
            {
                if (!Contains(extentPoint))
                {
                    edgesReached.AddRange(GetEdgesReached(extentPoint));
                }

                edgesReached = edgesReached.Distinct().ToList();

                // We are already overlapping all edges with 
                // the previous extents, so don't check the rest.
                if (edgesReached.Count == 4) break;
            }

            foreach (var dir in edgesReached)
            {
                var neighbor = _world.GetChunkNeighbor(Position, dir);

                if (neighbor == null)
                {
                    var blueprint = _world.GetBlueprintNeighbor(Position, dir);
                    blueprint.Register(space);
                }
                else if (!neighbor.Spaces.Contains(space))
                {
                    throw new InvalidOperationException($"A space overlaps with an existing chunk! Chunk {neighbor} and space {space}.");
                }
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
            if (extentPoint.X > TopRightCorner.X) edgesReached.Add(Vector2.right);
            if (extentPoint.Y > TopRightCorner.Y) edgesReached.Add(Vector2.up);

            return edgesReached;
        }

        private static readonly Utilities.Debug.Log _log = new Utilities.Debug.Log("Chunk Blueprint");
    }
}