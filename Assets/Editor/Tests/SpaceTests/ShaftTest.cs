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
    public class ShaftTest
    {
        private static ChunkBuilder _testChunk;

        [SetUp]
        public void ClearScene() => EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
        [SetUp]
        public void PrepareTestChunk() => _testChunk = new ChunkBuilder(Vector2.zero, 32);

        [Test]
        public void ShaftBuilderClampTest()
        {
            foreach (var alignment in (ShaftBuilder.ShaftAlignment[])System.Enum.GetValues(typeof(ShaftBuilder.ShaftAlignment)))
            {
                var clampLeftShaft = new ShaftBuilder(_testChunk)
                                        .SetStartingPoint(-Vector2.one, alignment)
                                        .AddBoundary(Directions.Left, 0)
                                        .Build();

                foreach (var extent in clampLeftShaft.Extents.Perimeter.Vertices)
                {
                    Assert.LessOrEqual(0, extent.X, $"Failed to clamp left for {alignment}, as one extent is at {extent}.");
                }

                var clampDownShaft = new ShaftBuilder(_testChunk)
                                        .SetStartingPoint(-Vector2.one, alignment)
                                        .AddBoundary(Directions.Down, 0)
                                        .Build();

                foreach (var extent in clampDownShaft.Extents.Perimeter.Vertices)
                {
                    Assert.LessOrEqual(0, extent.Y, $"Failed to clamp down for {alignment}, as one extent is at {extent}.");
                }

                var clampRightShaft = new ShaftBuilder(_testChunk)
                                         .SetStartingPoint(Vector2.one, alignment)
                                         .AddBoundary(Directions.Right, 0)
                                         .Build();

                foreach (var extent in clampRightShaft.Extents.Perimeter.Vertices)
                {
                    Assert.GreaterOrEqual(0, extent.X, $"Failed to clamp right for {alignment}, as one extent is at {extent}.");
                }

                var clampUpShaft = new ShaftBuilder(_testChunk)
                                      .SetStartingPoint(Vector2.one, alignment)
                                      .AddBoundary(Directions.Up, 0)
                                      .Build();

                foreach (var extent in clampUpShaft.Extents.Perimeter.Vertices)
                {
                    Assert.GreaterOrEqual(0, extent.Y, $"Failed to clamp up for {alignment}, as one extent is at {extent}.");
                }
            }
        }

        [Test]
        public void ShaftBuilderCutTest()
        {
            foreach (var alignment in (ShaftBuilder.ShaftAlignment[])System.Enum.GetValues(typeof(ShaftBuilder.ShaftAlignment)))
            {
                var cutRightShaft = new ShaftBuilder(_testChunk)
                                       .SetStartingPoint(-Vector2.one, alignment)
                                       .AddBoundary(Directions.Right, 0)
                                       .AddBoundary(Directions.Left, 0)
                                       .Build();

                foreach (var extent in cutRightShaft.Extents.Perimeter.Vertices)
                {
                    Assert.AreEqual(0, extent.X, $"Failed to cut right for {alignment}, as one extent is at {extent}.");
                }

                var cutUpShaft = new ShaftBuilder(_testChunk)
                                    .SetStartingPoint(-Vector2.one, alignment)
                                    .AddBoundary(Directions.Up, 0)
                                    .AddBoundary(Directions.Down, 0)
                                    .Build();

                foreach (var extent in cutUpShaft.Extents.Perimeter.Vertices)
                {
                    Assert.AreEqual(0, extent.Y, $"Failed to cut up for {alignment}, as one extent is at {extent}.");
                }

                var cutLeftShaft = new ShaftBuilder(_testChunk)
                                      .SetStartingPoint(Vector2.one, alignment)
                                      .AddBoundary(Directions.Left, 0)
                                      .AddBoundary(Directions.Right, 0)
                                      .Build();

                foreach (var extent in cutLeftShaft.Extents.Perimeter.Vertices)
                {
                    Assert.AreEqual(0, extent.X, $"Failed to cut left for {alignment}, as one extent is at {extent}.");
                }

                var cutDownShaft = new ShaftBuilder(_testChunk)
                                      .SetStartingPoint(Vector2.one, alignment)
                                      .AddBoundary(Directions.Down, 0)
                                      .AddBoundary(Directions.Up, 0)
                                      .Build();

                foreach (var extent in cutDownShaft.Extents.Perimeter.Vertices)
                {
                    Assert.AreEqual(0, extent.Y, $"Failed to cut down for {alignment}, as one extent is at {extent}.");
                }
            }
        }

        [Test]
        public void ShaftBuilderResizeTest()
        {
            foreach (var alignment in (ShaftBuilder.ShaftAlignment[])System.Enum.GetValues(typeof(ShaftBuilder.ShaftAlignment)))
            {
                var setHeightShaft = new ShaftBuilder(_testChunk)
                                        .SetHeight(10)
                                        .Build() as Tunnel;

                Assert.AreEqual(10, setHeightShaft.Height, $"Tunnel height was not as expected for alignment {alignment}.");

                var setLengthShaft = new ShaftBuilder(_testChunk)
                                        .SetWidth(10)
                                        .Build() as Tunnel;

                Assert.AreEqual(10, setLengthShaft.Width, $"Tunnel length was not as expected for alignment {alignment}.");
            }
        }

        [Test]
        public void ShaftGetBlockTest()
        {
            var shaft = new ShaftBuilder(_testChunk)
                           .Build() as Tunnel;

            for (var x = shaft.BottomLeftCorner.X; x <= shaft.TopRightCorner.X; x++)
            {
                for (var y = shaft.BottomLeftCorner.Y; y <= shaft.TopRightCorner.Y; y++)
                {
                    var block = shaft.GetBlockType(new IntVector2(x, y));
                    Assert.AreEqual(BlockTypes.None, block, $"Found the wrong block at [{x},{y}].");
                }
            }
        }
    }
}