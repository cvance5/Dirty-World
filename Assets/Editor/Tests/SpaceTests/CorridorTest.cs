using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using WorldObjects;
using WorldObjects.Blocks;
using WorldObjects.Spaces;
using WorldObjects.WorldGeneration;
using WorldObjects.WorldGeneration.EnemyGeneration;
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
        public void CorridorEnemyGenerationTest()
        {
            for (var repeat = 0; repeat < 1; repeat++)
            {
                var corridor = new CorridorBuilder(_testChunk)
                                  .SetLength(5)
                                  .SetHeight(5)
                                  .SetExtraRiskPoints(5)
                                  .SetAllowEnemies(true)
                                  .Build();

                var enemySpawns = corridor.EnemySpawns;

                Assert.IsNotEmpty(enemySpawns, $"No enemies were spawned.");

                foreach (var enemy in enemySpawns)
                {
                    Assert.True(corridor.Extents.Contains(enemy.Position), $"Enemy spawned outside of monster den.");
                    Assert.AreNotEqual(EnemyTypes.None, enemy.Type, $"Enemy spawned as `none`.");
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

                foreach (var extent in clampLeftCorridor.Extents.Perimeter.Vertices)
                {
                    Assert.LessOrEqual(0, extent.X, $"Failed to clamp left for {alignment}, as one extent is at {extent}.");
                }

                var clampDownCorridor = new CorridorBuilder(_testChunk)
                                           .SetStartingPoint(-Vector2.one, alignment)
                                           .AddBoundary(Directions.Down, 0)
                                           .Build();

                foreach (var extent in clampDownCorridor.Extents.Perimeter.Vertices)
                {
                    Assert.LessOrEqual(0, extent.Y, $"Failed to clamp down for {alignment}, as one extent is at {extent}.");
                }

                var clampRightCorridor = new CorridorBuilder(_testChunk)
                                            .SetStartingPoint(Vector2.one, alignment)
                                            .AddBoundary(Directions.Right, 0)
                                            .Build();

                foreach (var extent in clampRightCorridor.Extents.Perimeter.Vertices)
                {
                    Assert.GreaterOrEqual(0, extent.X, $"Failed to clamp right for {alignment}, as one extent is at {extent}.");
                }

                var clampUpCorridor = new CorridorBuilder(_testChunk)
                                         .SetStartingPoint(Vector2.one, alignment)
                                         .AddBoundary(Directions.Up, 0)
                                         .Build();

                foreach (var extent in clampUpCorridor.Extents.Perimeter.Vertices)
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

                foreach (var extent in cutRightCorridor.Extents.Perimeter.Vertices)
                {
                    Assert.AreEqual(0, extent.X, $"Failed to cut right for {alignment}, as one extent is at {extent}.");
                }

                var cutUpCorridor = new CorridorBuilder(_testChunk)
                                           .SetStartingPoint(-Vector2.one, alignment)
                                           .AddBoundary(Directions.Up, 0)
                                           .AddBoundary(Directions.Down, 0)
                                           .Build();

                foreach (var extent in cutUpCorridor.Extents.Perimeter.Vertices)
                {
                    Assert.AreEqual(0, extent.Y, $"Failed to cut up for {alignment}, as one extent is at {extent}.");
                }

                var cutLeftCorridor = new CorridorBuilder(_testChunk)
                                            .SetStartingPoint(Vector2.one, alignment)
                                            .AddBoundary(Directions.Left, 0)
                                            .AddBoundary(Directions.Right, 0)
                                            .Build();

                foreach (var extent in cutLeftCorridor.Extents.Perimeter.Vertices)
                {
                    Assert.AreEqual(0, extent.X, $"Failed to cut left for {alignment}, as one extent is at {extent}.");
                }

                var cutDownCorridor = new CorridorBuilder(_testChunk)
                                         .SetStartingPoint(Vector2.one, alignment)
                                         .AddBoundary(Directions.Down, 0)
                                         .AddBoundary(Directions.Up, 0)
                                         .Build();

                foreach (var extent in cutDownCorridor.Extents.Perimeter.Vertices)
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
                                            .Build() as Tunnel;

                Assert.AreEqual(10, setHeightCorridor.Height, $"Tunnel height was not as expected for alignment {alignment}.");

                var setLengthCorridor = new CorridorBuilder(_testChunk)
                                            .SetLength(10)
                                            .Build() as Tunnel;

                Assert.AreEqual(10, setLengthCorridor.Width, $"Tunnel width was not as expected for alignment {alignment}.");
            }
        }

        [Test]
        public void CorridorGetBlockTest()
        {
            var corridor = new CorridorBuilder(_testChunk)
                               .Build() as Tunnel;

            for (var x = corridor.BottomLeftCorner.X; x <= corridor.TopRightCorner.X; x++)
            {
                for (var y = corridor.BottomLeftCorner.Y; y <= corridor.TopRightCorner.Y; y++)
                {
                    var block = corridor.GetBlockType(new IntVector2(x, y));
                    Assert.AreEqual(BlockTypes.None, block, $"Found the wrong block at [{x},{y}].");
                }
            }
        }
    }
}