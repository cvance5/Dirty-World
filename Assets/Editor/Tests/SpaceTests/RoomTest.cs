using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using WorldObjects;
using WorldObjects.Blocks;
using WorldObjects.Spaces;
using WorldObjects.WorldGeneration;
using WorldObjects.WorldGeneration.SpaceGeneration;

namespace Tests.SpaceTests
{
    public class RoomTest
    {
        private static ChunkBuilder _testChunk;

        [SetUp]
        public void ClearScene() => EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
        [SetUp]
        public void PrepareTestChunk() => _testChunk = new ChunkBuilder(Vector2.zero, 32);

        [Test]
        public void RoomContainsTest()
        {
            var room = new Room(new IntVector2(-2, -2), new IntVector2(2, 2));

            for (var x = -5; x <= 5; x++)
            {
                for (var y = -5; y <= 5; y++)
                {
                    var expectation = y >= -2 &&
                                      y <= 2 &&
                                      x >= -2
                                      && x <= 2;

                    var actual = room.Contains(new IntVector2(x, y));

                    Assert.AreEqual(actual, expectation, $"Position: [{x},{y}] | Expected: {expectation} | Actual: {actual}");
                }
            }
        }

        [Test]
        public void RoomBuilderClampTest()
        {
            var clampLeftRoom = new RoomBuilder(_testChunk)
                                   .SetCenter(-Vector2.one)
                                   .AddBoundary(Directions.Left, 0)
                                   .Build();

            foreach (var extent in clampLeftRoom.Extents)
            {
                Assert.GreaterOrEqual(extent.X, 0, $"Failed to clamp left, as one extent is at {extent}.");
            }

            var clampDownRoom = new RoomBuilder(_testChunk)
                                   .SetCenter(-Vector2.one)
                                   .AddBoundary(Directions.Down, 0)
                                   .Build();

            foreach (var extent in clampDownRoom.Extents)
            {
                Assert.GreaterOrEqual(extent.Y, 0, $"Failed to clamp down, as one extent is at {extent}.");
            }

            var clampRightRoom = new RoomBuilder(_testChunk)
                                    .SetCenter(Vector2.one)
                                    .AddBoundary(Directions.Right, 0)
                                    .Build();

            foreach (var extent in clampRightRoom.Extents)
            {
                Assert.LessOrEqual(extent.X, 0, $"Failed to clamp right, as one extent is at {extent}.");
            }

            var clampUpRoom = new RoomBuilder(_testChunk)
                                 .SetCenter(Vector2.one)
                                 .AddBoundary(Directions.Up, 0)
                                 .Build();

            foreach (var extent in clampUpRoom.Extents)
            {
                Assert.LessOrEqual(extent.Y, 0, $"Failed to clamp up, as one extent is at {extent}.");
            }
        }

        [Test]
        public void RoomBuilderCutTest()
        {
            var cutRightRoom = new RoomBuilder(_testChunk)
                                  .SetCenter(-Vector2.one)
                                  .SetSize(5)
                                  .AddBoundary(Directions.Right, 0)
                                  .AddBoundary(Directions.Left, 0)
                                  .Build();

            foreach (var extent in cutRightRoom.Extents)
            {
                Assert.AreEqual(extent.X, 0, $"Failed to cut right, as one extent is at {extent}.");
            }

            var cutUpRoom = new RoomBuilder(_testChunk)
                               .SetCenter(-Vector2.one)
                               .SetSize(5)
                               .AddBoundary(Directions.Up, 0)
                               .AddBoundary(Directions.Down, 0)
                               .Build();

            foreach (var extent in cutUpRoom.Extents)
            {
                Assert.AreEqual(extent.Y, 0, $"Failed to cut up, as one extent is at {extent}.");
            }

            var cutLeftRoom = new RoomBuilder(_testChunk)
                                 .SetCenter(Vector2.one)
                                 .SetSize(5)
                                 .AddBoundary(Directions.Left, 0)
                                 .AddBoundary(Directions.Right, 0)
                                 .Build();

            foreach (var extent in cutRightRoom.Extents)
            {
                Assert.AreEqual(extent.X, 0, $"Failed to cut left, as one extent is at {extent}.");
            }

            var cutDownRoom = new RoomBuilder(_testChunk)
                                 .SetCenter(Vector2.one)
                                 .SetSize(5)
                                 .AddBoundary(Directions.Down, 0)
                                 .AddBoundary(Directions.Up, 0)
                                 .Build();

            foreach (var extent in cutUpRoom.Extents)
            {
                Assert.AreEqual(extent.Y, 0, $"Failed to cut down, as one extent is at {extent}.");
            }
        }

        [Test]
        public void RoomBuilderResizeTest()
        {
            var setHeightRoom = new RoomBuilder(_testChunk)
                                   .SetSize(10)
                                   .Build() as Room;

            Assert.AreEqual(10, setHeightRoom.TopRightCorner.Y - setHeightRoom.BottomLeftCorner.Y, $"Room height was not as expected.");
        }

        [Test]
        public void RoomGetBlockTest()
        {
            var room = new RoomBuilder(_testChunk)
                          .Build() as Room;

            for (var x = room.BottomLeftCorner.X; x <= room.TopRightCorner.X; x++)
            {
                for (var y = room.BottomLeftCorner.Y; y <= room.TopRightCorner.Y; y++)
                {
                    var block = room.GetBlockType(new IntVector2(x, y));
                    Assert.AreEqual(BlockTypes.None, block, $"Found the wrong block at [{x},{y}].");
                }
            }
        }
    }
}