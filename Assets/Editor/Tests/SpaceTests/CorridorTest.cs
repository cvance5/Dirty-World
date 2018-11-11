using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using WorldObjects;
using WorldObjects.Blocks;
using WorldObjects.Hazards;
using WorldObjects.Spaces;
using WorldObjects.WorldGeneration;
using WorldObjects.WorldGeneration.SpaceGeneration;

namespace Tests.SpaceTests
{
    public class CorridorTest
    {
        private static ChunkBuilder _testChunk;

        [SetUp]
        public void ClearScene() => EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
        [SetUp]
        public void PrepareTestChunk() => _testChunk = new ChunkBuilder(Vector2.zero, 32);


        [Test]
        public void CorridorContainsTest()
        {
            var corridor = new Corridor(new IntVector2(-2, -2), new IntVector2(2, 2), false);

            for (var x = -5; x <= 5; x++)
            {
                for (var y = -5; y <= 5; y++)
                {
                    var expectation = y >= -2 &&
                                      y <= 2 &&
                                      x >= -2
                                      && x <= 2;

                    var actual = corridor.Contains(new IntVector2(x, y));

                    Assert.AreEqual(actual, expectation, $"Position: [{x},{y}] | Expected: {expectation} | Actual: {actual}");
                }
            }
        }

        [Test]
        public void CorridorBuilderClampTest()
        {
            foreach (var alignment in (CorridorBuilder.CorridorAlignment[])System.Enum.GetValues(typeof(CorridorBuilder.CorridorAlignment)))
            {
                var clampLeftCorridor = new CorridorBuilder(_testChunk)
                                           .SetStartingPoint(-Vector2.one, alignment)
                                           .AddBoundary(Directions.Left, 0)
                                           .Build();

                foreach (var extent in clampLeftCorridor.Extents)
                {
                    Assert.LessOrEqual(0, extent.X, $"Failed to clamp left for {alignment}, as one extent is at {extent}.");
                }

                var clampDownCorridor = new CorridorBuilder(_testChunk)
                                           .SetStartingPoint(-Vector2.one, alignment)
                                           .AddBoundary(Directions.Down, 0)
                                           .Build();

                foreach (var extent in clampDownCorridor.Extents)
                {
                    Assert.LessOrEqual(0, extent.Y, $"Failed to clamp down for {alignment}, as one extent is at {extent}.");
                }

                var clampRightCorridor = new CorridorBuilder(_testChunk)
                                            .SetStartingPoint(Vector2.one, alignment)
                                            .AddBoundary(Directions.Right, 0)
                                            .Build();

                foreach (var extent in clampRightCorridor.Extents)
                {
                    Assert.GreaterOrEqual(0, extent.X, $"Failed to clamp right for {alignment}, as one extent is at {extent}.");
                }

                var clampUpCorridor = new CorridorBuilder(_testChunk)
                                         .SetStartingPoint(Vector2.one, alignment)
                                         .AddBoundary(Directions.Up, 0)
                                         .Build();

                foreach (var extent in clampUpCorridor.Extents)
                {
                    Assert.GreaterOrEqual(0, extent.Y, $"Failed to clamp up for {alignment}, as one extent is at {extent}.");
                }
            }
        }

        [Test]
        public void CorridorBuilderCutTest()
        {
            foreach (var alignment in (CorridorBuilder.CorridorAlignment[])System.Enum.GetValues(typeof(CorridorBuilder.CorridorAlignment)))
            {
                var cutRightCorridor = new CorridorBuilder(_testChunk)
                                           .SetStartingPoint(-Vector2.one, alignment)
                                           .AddBoundary(Directions.Right, 0)
                                           .AddBoundary(Directions.Left, 0)
                                           .Build();

                foreach (var extent in cutRightCorridor.Extents)
                {
                    Assert.AreEqual(0, extent.X, $"Failed to cut right for {alignment}, as one extent is at {extent}.");
                }

                var cutUpCorridor = new CorridorBuilder(_testChunk)
                                           .SetStartingPoint(-Vector2.one, alignment)
                                           .AddBoundary(Directions.Up, 0)
                                           .AddBoundary(Directions.Down, 0)
                                           .Build();

                foreach (var extent in cutUpCorridor.Extents)
                {
                    Assert.AreEqual(0, extent.Y, $"Failed to cut up for {alignment}, as one extent is at {extent}.");
                }

                var cutLeftCorridor = new CorridorBuilder(_testChunk)
                                            .SetStartingPoint(Vector2.one, alignment)
                                            .AddBoundary(Directions.Left, 0)
                                            .AddBoundary(Directions.Right, 0)
                                            .Build();

                foreach (var extent in cutLeftCorridor.Extents)
                {
                    Assert.AreEqual(0, extent.X, $"Failed to cut left for {alignment}, as one extent is at {extent}.");
                }

                var cutDownCorridor = new CorridorBuilder(_testChunk)
                                         .SetStartingPoint(Vector2.one, alignment)
                                         .AddBoundary(Directions.Down, 0)
                                         .AddBoundary(Directions.Up, 0)
                                         .Build();

                foreach (var extent in cutDownCorridor.Extents)
                {
                    Assert.AreEqual(0, extent.Y, $"Failed to cut down for {alignment}, as one extent is at {extent}.");
                }
            }
        }

        [Test]
        public void CorridorBuilderResizeTest()
        {
            foreach (var alignment in (CorridorBuilder.CorridorAlignment[])System.Enum.GetValues(typeof(CorridorBuilder.CorridorAlignment)))
            {
                var setHeightCorridor = new CorridorBuilder(_testChunk)
                                            .SetHeight(10)
                                            .Build() as Corridor;

                Assert.AreEqual(10, setHeightCorridor.Height, $"Corridor height was not as expected for alignment {alignment}.");

                var setLengthCorridor = new CorridorBuilder(_testChunk)
                                            .SetLength(10)
                                            .Build() as Corridor;

                Assert.AreEqual(10, setLengthCorridor.Length, $"Corridor length was not as expected for alignment {alignment}.");
            }
        }

        [Test]
        public void CorridorBuilderSetHazardsTest()
        {
            var setHazardousCorridor = new CorridorBuilder(_testChunk)
                                           .SetHazards(true)
                                           .Build() as Corridor;

            for (var y = setHazardousCorridor.BottomLeftCorner.Y; y <= setHazardousCorridor.TopRightCorner.Y; y++)
            {
                for (var x = setHazardousCorridor.BottomLeftCorner.X; x <= setHazardousCorridor.TopRightCorner.X; x++)
                {
                    if (y == setHazardousCorridor.BottomLeftCorner.Y)
                    {
                        Assert.AreEqual(HazardTypes.Stalagmite, setHazardousCorridor.GetHazard(new IntVector2(x, y)), $"No spikes found at [{x},{y}].");
                    }
                    else
                    {
                        Assert.AreEqual(HazardTypes.None, setHazardousCorridor.GetHazard(new IntVector2(x, y)), $"Spikes found at [{x},{y}].");
                    }
                }
            }
        }

        [Test]
        public void CorridorGetBlockTest()
        {
            var corridor = new CorridorBuilder(_testChunk)
                               .SetHazards(false)
                               .Build() as Corridor;

            for (var x = corridor.BottomLeftCorner.X; x <= corridor.TopRightCorner.X; x++)
            {
                for (var y = corridor.BottomLeftCorner.Y; y <= corridor.TopRightCorner.Y; y++)
                {
                    var block = corridor.GetBlock(new IntVector2(x, y));
                    Assert.AreEqual(BlockTypes.None, block, $"Found the wrong block at [{x},{y}].");
                }
            }
        }
    }
}