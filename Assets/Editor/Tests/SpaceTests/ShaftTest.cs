using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using WorldObjects;
using WorldObjects.Blocks;
using WorldObjects.Spaces;
using WorldObjects.WorldGeneration;

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
        public void ShaftContainsTest()
        {
            var shaft = new Shaft(new IntVector2(-2, -2), new IntVector2(2, 2), false);

            for (var x = -5; x <= 5; x++)
            {
                for (var y = -5; y <= 5; y++)
                {
                    var expectation = y >= -2 &&
                                      y <= 2 &&
                                      x >= -2
                                      && x <= 2;

                    var actual = shaft.Contains(new IntVector2(x, y));

                    Assert.AreEqual(actual, expectation, $"Position: [{x},{y}] | Expected: {expectation} | Actual: {actual}");
                }
            }
        }

        [Test]
        public void ShaftBuilderClampTest()
        {
            foreach (var alignment in (ShaftBuilder.ShaftAlignment[])System.Enum.GetValues(typeof(ShaftBuilder.ShaftAlignment)))
            {
                var clampLeftShaft = new ShaftBuilder(_testChunk)
                                        .SetStartingPoint(-Vector2.one, alignment)
                                        .AddBoundary(Directions.Left, 0)
                                        .Build();

                foreach (var extent in clampLeftShaft.Extents)
                {
                    Assert.LessOrEqual(0, extent.X, $"Failed to clamp left for {alignment}, as one extent is at {extent}.");
                }

                var clampDownShaft = new ShaftBuilder(_testChunk)
                                        .SetStartingPoint(-Vector2.one, alignment)
                                        .AddBoundary(Directions.Down, 0)
                                        .Build();

                foreach (var extent in clampDownShaft.Extents)
                {
                    Assert.LessOrEqual(0, extent.Y, $"Failed to clamp down for {alignment}, as one extent is at {extent}.");
                }

                var clampRightShaft = new ShaftBuilder(_testChunk)
                                         .SetStartingPoint(Vector2.one, alignment)
                                         .AddBoundary(Directions.Right, 0)
                                         .Build();

                foreach (var extent in clampRightShaft.Extents)
                {
                    Assert.GreaterOrEqual(0, extent.X, $"Failed to clamp right for {alignment}, as one extent is at {extent}.");
                }

                var clampUpShaft = new ShaftBuilder(_testChunk)
                                      .SetStartingPoint(Vector2.one, alignment)
                                      .AddBoundary(Directions.Up, 0)
                                      .Build();

                foreach (var extent in clampUpShaft.Extents)
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

                foreach (var extent in cutRightShaft.Extents)
                {
                    Assert.AreEqual(0, extent.X, $"Failed to cut right for {alignment}, as one extent is at {extent}.");
                }

                var cutUpShaft = new ShaftBuilder(_testChunk)
                                        .SetStartingPoint(-Vector2.one, alignment)
                                        .AddBoundary(Directions.Up, 0)
                                        .AddBoundary(Directions.Down, 0)
                                        .Build();

                foreach (var extent in cutUpShaft.Extents)
                {
                    Assert.AreEqual(0, extent.Y, $"Failed to cut up for {alignment}, as one extent is at {extent}.");
                }

                var cutLeftShaft = new ShaftBuilder(_testChunk)
                                         .SetStartingPoint(Vector2.one, alignment)
                                         .AddBoundary(Directions.Left, 0)
                                         .AddBoundary(Directions.Right, 0)
                                         .Build();

                foreach (var extent in cutLeftShaft.Extents)
                {
                    Assert.AreEqual(0, extent.X, $"Failed to cut left for {alignment}, as one extent is at {extent}.");
                }

                var cutDownShaft = new ShaftBuilder(_testChunk)
                                      .SetStartingPoint(Vector2.one, alignment)
                                      .AddBoundary(Directions.Down, 0)
                                      .AddBoundary(Directions.Up, 0)
                                      .Build();

                foreach (var extent in cutDownShaft.Extents)
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
                                            .Build() as Shaft;

                Assert.AreEqual(10, setHeightShaft.Height, $"Shaft height was not as expected for alignment {alignment}.");

                var setLengthShaft = new ShaftBuilder(_testChunk)
                                            .SetWidth(10)
                                            .Build() as Shaft;

                Assert.AreEqual(10, setLengthShaft.Width, $"Shaft length was not as expected for alignment {alignment}.");
            }
        }

        [Test]
        public void ShaftGetBlockTest()
        {
            var shaft = new ShaftBuilder(_testChunk)
                               .Build() as Shaft;

            for (var x = shaft.BottomLeftCorner.X; x <= shaft.TopRightCorner.X; x++)
            {
                for (var y = shaft.BottomLeftCorner.Y; y <= shaft.TopRightCorner.Y; y++)
                {
                    var block = shaft.GetBlock(new IntVector2(x, y));

                    if (y == shaft.TopRightCorner.Y)
                    {
                        Assert.AreNotEqual(BlockTypes.None, block, $"Didn't find a block at [{x},{y}].");
                    }
                    else
                    {
                        Assert.AreEqual(BlockTypes.None, block, $"Found the wrong block at [{x},{y}].");
                    }
                }
            }
        }
    }
}