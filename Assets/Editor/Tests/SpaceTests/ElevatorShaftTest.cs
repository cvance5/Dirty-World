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
    public class ElevatorShaftTest
    {
        private static ChunkBuilder _testChunk;

        [SetUp]
        public void ClearScene() => EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
        [SetUp]
        public void PrepareTestChunk() => _testChunk = new ChunkBuilder(Vector2.zero, 32);

        [Test]
        public void ElevatorShaftContainsTest()
        {
            var shaft = new ElevatorShaft(new IntVector2(-2, -2), new IntVector2(2, 2), false, null);

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
        public void ElevatorShaftBuilderClampTest()
        {
            foreach (var alignment in (ShaftBuilder.ShaftAlignment[])System.Enum.GetValues(typeof(ShaftBuilder.ShaftAlignment)))
            {
                var clampLeftShaft = new ElevatorShaftBuilder(_testChunk)
                                        .SetStartingPoint(-Vector2.one, alignment)
                                        .AddBoundary(Directions.Left, 0)
                                        .Build();

                foreach (var extent in clampLeftShaft.Extents)
                {
                    Assert.LessOrEqual(0, extent.X, $"Failed to clamp left for {alignment}, as one extent is at {extent}.");
                }

                var clampDownShaft = new ElevatorShaftBuilder(_testChunk)
                                        .SetStartingPoint(-Vector2.one, alignment)
                                        .AddBoundary(Directions.Down, 0)
                                        .Build();

                foreach (var extent in clampDownShaft.Extents)
                {
                    Assert.LessOrEqual(0, extent.Y, $"Failed to clamp down for {alignment}, as one extent is at {extent}.");
                }

                var clampRightShaft = new ElevatorShaftBuilder(_testChunk)
                                         .SetStartingPoint(Vector2.one, alignment)
                                         .AddBoundary(Directions.Right, 0)
                                         .Build();

                foreach (var extent in clampRightShaft.Extents)
                {
                    Assert.GreaterOrEqual(0, extent.X, $"Failed to clamp right for {alignment}, as one extent is at {extent}.");
                }

                var clampUpShaft = new ElevatorShaftBuilder(_testChunk)
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
        public void ElevatorShaftBuilderCutTest()
        {
            foreach (var alignment in (ShaftBuilder.ShaftAlignment[])System.Enum.GetValues(typeof(ShaftBuilder.ShaftAlignment)))
            {
                var cutRightShaft = new ElevatorShaftBuilder(_testChunk)
                                       .SetStartingPoint(-Vector2.one, alignment)
                                       .AddBoundary(Directions.Right, 0)
                                       .AddBoundary(Directions.Left, 0)
                                       .Build();

                foreach (var extent in cutRightShaft.Extents)
                {
                    Assert.AreEqual(0, extent.X, $"Failed to cut right for {alignment}, as one extent is at {extent}.");
                }

                var cutUpShaft = new ElevatorShaftBuilder(_testChunk)
                                    .SetNumberOfStories(7)
                                    .SetStoryHeight(9)
                                    .SetStartingPoint(-Vector2.one, alignment)
                                    .AddBoundary(Directions.Up, 2)
                                    .AddBoundary(Directions.Down, 0)
                                    .Build();

                foreach (var extent in cutUpShaft.Extents)
                {
                    Assert.AreEqual(0, extent.Y, $"Failed to cut up for {alignment}, as one extent is at {extent}.");
                }

                var cutLeftShaft = new ElevatorShaftBuilder(_testChunk)
                                      .SetStartingPoint(Vector2.one, alignment)
                                      .AddBoundary(Directions.Left, 0)
                                      .AddBoundary(Directions.Right, 0)
                                      .Build();

                foreach (var extent in cutLeftShaft.Extents)
                {
                    Assert.AreEqual(0, extent.X, $"Failed to cut left for {alignment}, as one extent is at {extent}.");
                }

                var cutDownShaft = new ElevatorShaftBuilder(_testChunk)
                                      .SetNumberOfStories(7)
                                      .SetStoryHeight(9)
                                      .SetStartingPoint(Vector2.one, alignment)
                                      .AddBoundary(Directions.Down, 2)
                                      .AddBoundary(Directions.Up, 0)
                                      .Build();

                foreach (var extent in cutDownShaft.Extents)
                {
                    Assert.AreEqual(0, extent.Y, $"Failed to cut down for {alignment}, as one extent is at {extent}.");
                }
            }
        }

        [Test]
        public void ElevatorShaftHeightFieldsTest()
        {
            var setHeightBuilder = new ElevatorShaftBuilder(_testChunk)
                                      .SetNumberOfStories(1)
                                      .SetStoryHeight(10)
                                      .SetHeight(10) as ElevatorShaftBuilder;

            var defaultValueShaft = setHeightBuilder.Build() as ElevatorShaft;
            Assert.AreEqual(10, defaultValueShaft.Height, $"Shaft height was changed unexpectedly.");

            setHeightBuilder.SetNumberOfStories(2);

            var setStoriesShaft = setHeightBuilder.Build() as ElevatorShaft;
            Assert.AreEqual(20, setStoriesShaft.Height, $"Shaft height did not scale when stories changed.");

            setHeightBuilder.SetNumberOfStories(1)
                            .SetStoryHeight(20);

            var setStoryHeightShaft = setHeightBuilder.Build() as ElevatorShaft;
            Assert.AreEqual(20, setStoryHeightShaft.Height, $"Shaft height did not scale when story height changed.");

            setHeightBuilder.SetNumberOfStories(3)
                            .SetStoryHeight(5)
                            .SetHeight(20);

            var setHeightErraticallyShaft = setHeightBuilder.Build() as ElevatorShaft;
            Assert.AreEqual(20, setHeightErraticallyShaft.Height, $"Shaft height did not set correctly with acceptable values.");

            setHeightBuilder.SetHeight(8);

            var setHeightInvalidShaft = setHeightBuilder.Build() as ElevatorShaft;
            Assert.AreNotEqual(8, setHeightInvalidShaft, $"Shaft height should not be set to an impossible value.");
        }

        [Test]
        public void ElevatorShaftBuilderResizeTest()
        {
            foreach (var alignment in (ShaftBuilder.ShaftAlignment[])System.Enum.GetValues(typeof(ShaftBuilder.ShaftAlignment)))
            {
                var setHeightShaft = new ElevatorShaftBuilder(_testChunk)
                                        .SetStoryHeight(5)
                                        .SetNumberOfStories(2)
                                        .SetHeight(10)
                                        .Build() as Shaft;

                Assert.AreEqual(10, setHeightShaft.Height, $"Shaft height was not as expected for alignment {alignment}.");

                var setLengthShaft = new ElevatorShaftBuilder(_testChunk)
                                        .SetWidth(10)
                                        .Build() as Shaft;

                Assert.AreEqual(10, setLengthShaft.Width, $"Shaft length was not as expected for alignment {alignment}.");
            }
        }

        [Test]
        public void ElevatorShaftGetBlockTest()
        {
            var shaft = new ElevatorShaftBuilder(_testChunk)
                           .Build() as Shaft;

            for (var x = shaft.BottomLeftCorner.X; x <= shaft.TopRightCorner.X; x++)
            {
                for (var y = shaft.BottomLeftCorner.Y; y <= shaft.TopRightCorner.Y; y++)
                {
                    var block = shaft.GetBlockType(new IntVector2(x, y));

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
