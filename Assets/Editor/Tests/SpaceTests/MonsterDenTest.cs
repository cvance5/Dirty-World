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
    public class MonsterDenTest
    {
        private static ChunkBuilder _testChunk;

        [SetUp]
        public void ClearScene() => EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
        [SetUp]
        public void PrepareTestChunk() => _testChunk = new ChunkBuilder(Vector2.zero, 32);

        [Test]
        public void MonsterDenContainsTest()
        {
            var monsterDen = new MonsterDen(Vector2.zero, 3);

            var containsBottomLeft = monsterDen.Extents.Contains(new IntVector2(-3, 0));
            Assert.True(containsBottomLeft, "Monster den does not contain it's bottom-left-most point.");

            var containsMidLeft = monsterDen.Extents.Contains(new IntVector2(-2, 1));
            Assert.True(containsMidLeft, "Monster den does not contain it's mid-left point.");

            var containsTopMiddle = monsterDen.Extents.Contains(new IntVector2(0, 3));
            Assert.True(containsTopMiddle, "Monster den does not contain it's top point.");

            var containsMidRight = monsterDen.Extents.Contains(new IntVector2(1, 2));
            Assert.True(containsMidRight, "Monster den does not contain it's mid-right point.");

            var containsBottomRight = monsterDen.Extents.Contains(new IntVector2(3, 0));
            Assert.True(containsBottomRight, "Monster den does not contain it's bottom-right-most point.");

            var containsBottomMiddle = monsterDen.Extents.Contains(new IntVector2(0, 0));
            Assert.True(containsBottomMiddle, "Monster den does not contain it's bottom-middle point.");

            var doesNotContainFarLeft = monsterDen.Extents.Contains(new IntVector2(-4, 0));
            Assert.False(doesNotContainFarLeft, "Monster den contains a point too far left.");

            var doesNotContainMidLeft = monsterDen.Extents.Contains(new IntVector2(-2, 2));
            Assert.False(doesNotContainMidLeft, "Monster den contains a point above its left leg.");

            var doesNotContainFarUp = monsterDen.Extents.Contains(new IntVector2(0, 4));
            Assert.False(doesNotContainFarUp, "Monster den contains a point too far up.");

            var doesNotContainMidRight = monsterDen.Extents.Contains(new IntVector2(1, 3));
            Assert.False(doesNotContainMidRight, "Monster den contains a point above its right leg.");

            var doesNotContainFarRight = monsterDen.Extents.Contains(new IntVector2(4, 0));
            Assert.False(doesNotContainFarRight, "Monster den contains a point too far right.");

            var doesNotContainFarDown = monsterDen.Extents.Contains(new IntVector2(0, -1));
            Assert.False(doesNotContainFarDown, "Monster den contains a point too far down.");
        }

        [Test]
        public void MonsterDenEnemyGenerationTest()
        {
            for (var repeat = 0; repeat < 50; repeat++)
            {
                var monsterDen = new MonsterDenBuilder(_testChunk)
                                    .SetCenterpoint(Vector2.zero)
                                    .SetRadius(5)
                                    .SetExtraRiskPoints(5)
                                    .Build();

                var enemySpawns = monsterDen.EnemySpawns;

                Assert.IsNotEmpty(enemySpawns, $"No enemies were spawned.");

                foreach (var enemy in enemySpawns)
                {
                    Assert.True(monsterDen.Extents.Contains(enemy.Position), $"Enemy spawned outside of monster den.");
                    Assert.AreNotEqual(EnemyTypes.None, enemy.Type, $"Enemy spawned as `none`.");
                }
            }
        }

        [Test]
        public void MonsterDenBuilderClampTest()
        {
            var clampLeftMonsterDen = new MonsterDenBuilder(_testChunk)
                                         .SetCenterpoint(-Vector2.one)
                                         .AddBoundary(Directions.Left, 0)
                                         .Build();

            foreach (var extent in clampLeftMonsterDen.Extents.Perimeter.Vertices)
            {
                Assert.GreaterOrEqual(extent.X, 0, $"Failed to clamp left, as one extent is at {extent}.");
            }

            var clampDownMonsterDen = new MonsterDenBuilder(_testChunk)
                                         .SetCenterpoint(-Vector2.one)
                                         .AddBoundary(Directions.Down, 0)
                                         .Build();

            foreach (var extent in clampDownMonsterDen.Extents.Perimeter.Vertices)
            {
                Assert.GreaterOrEqual(extent.Y, 0, $"Failed to clamp down, as one extent is at {extent}.");
            }

            var clampRightMonsterDen = new MonsterDenBuilder(_testChunk)
                                          .SetCenterpoint(Vector2.one)
                                          .AddBoundary(Directions.Right, 0)
                                          .Build();

            foreach (var extent in clampRightMonsterDen.Extents.Perimeter.Vertices)
            {
                Assert.LessOrEqual(extent.X, 0, $"Failed to clamp right, as one extent is at {extent}.");
            }

            var clampUpMonsterDen = new MonsterDenBuilder(_testChunk)
                                       .SetCenterpoint(Vector2.one)
                                       .AddBoundary(Directions.Up, 0)
                                       .Build();

            foreach (var extent in clampUpMonsterDen.Extents.Perimeter.Vertices)
            {
                Assert.LessOrEqual(extent.Y, 0, $"Failed to clamp up, as one extent is at {extent}.");
            }
        }

        [Test]
        public void MonsterDenBuilderCutTest()
        {
            var cutRightMonsterDen = new MonsterDenBuilder(_testChunk)
                                        .SetCenterpoint(-Vector2.one)
                                        .SetRadius(5)
                                        .AddBoundary(Directions.Right, 0)
                                        .AddBoundary(Directions.Left, 0)
                                        .Build();

            foreach (var extent in cutRightMonsterDen.Extents.Perimeter.Vertices)
            {
                Assert.AreEqual(extent.X, 0, $"Failed to cut right, as one extent is at {extent}.");
            }

            var cutUpMonsterDen = new MonsterDenBuilder(_testChunk)
                                     .SetCenterpoint(-Vector2.one)
                                     .SetRadius(5)
                                     .AddBoundary(Directions.Up, 0)
                                     .AddBoundary(Directions.Down, 0)
                                     .Build();

            foreach (var extent in cutUpMonsterDen.Extents.Perimeter.Vertices)
            {
                Assert.AreEqual(extent.Y, 0, $"Failed to cut up, as one extent is at {extent}.");
            }

            var cutLeftMonsterDen = new MonsterDenBuilder(_testChunk)
                                       .SetCenterpoint(Vector2.one)
                                       .SetRadius(5)
                                       .AddBoundary(Directions.Left, 0)
                                       .AddBoundary(Directions.Right, 0)
                                       .Build();

            foreach (var extent in cutRightMonsterDen.Extents.Perimeter.Vertices)
            {
                Assert.AreEqual(extent.X, 0, $"Failed to cut left, as one extent is at {extent}.");
            }

            var cutDownMonsterDen = new MonsterDenBuilder(_testChunk)
                                       .SetCenterpoint(Vector2.one)
                                       .SetRadius(5)
                                       .AddBoundary(Directions.Down, 0)
                                       .AddBoundary(Directions.Up, 0)
                                       .Build();

            foreach (var extent in cutUpMonsterDen.Extents.Perimeter.Vertices)
            {
                Assert.AreEqual(extent.Y, 0, $"Failed to cut down, as one extent is at {extent}.");
            }
        }

        [Test]
        public void MonsterDenBuilderResizeTest()
        {
            var setHeightMonsterDen = new MonsterDenBuilder(_testChunk)
                                        .SetRadius(10)
                                        .Build() as MonsterDen;

            Assert.AreEqual(10, setHeightMonsterDen.Radius, $"MonsterDen height was not as expected.");
        }

        [Test]
        public void MonsterDenGetBlockTest()
        {
            var monsterDen = new MonsterDenBuilder(_testChunk)
                                .Build() as MonsterDen;

            for (var x = monsterDen.Centerpoint.X - monsterDen.Radius; x <= monsterDen.Centerpoint.X + monsterDen.Radius; x++)
            {
                var offsetFromCenter = Mathf.Abs(x - monsterDen.Centerpoint.X);

                for (var y = monsterDen.Centerpoint.Y; y <= monsterDen.Centerpoint.Y + monsterDen.Radius - offsetFromCenter; y++)
                {
                    var block = monsterDen.GetBlockType(new IntVector2(x, y));
                    Assert.AreEqual(block, BlockTypes.None, $"Found the wrong block at [{x},{y}].");
                }
            }
        }
    }
}