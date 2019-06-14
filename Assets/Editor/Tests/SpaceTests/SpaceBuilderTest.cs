using MathConcepts;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using WorldObjects.WorldGeneration;
using WorldObjects.WorldGeneration.SpaceGeneration;

namespace Tests.SpaceTests
{
    public class SpaceBuilderTest
    {
        private static ChunkBuilder _testChunk;

        private static readonly List<Type> _builderTypes = new List<Type>()
        {
            typeof(TunnelBuilder),
            typeof(CorridorBuilder),
            typeof(ShaftBuilder),
            typeof(ElevatorShaftBuilder),
            typeof(RoomBuilder),
            typeof(MonsterDenBuilder)
            //typeof(PlexusBuilder)
        };

        [SetUp]
        public void ClearScene() => EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
        [SetUp]
        public void PrepareTestChunk() => _testChunk = new ChunkBuilder(Vector2.zero, 32);

        [Test]
        public void ClampTest()
        {
            for (var run = 0; run < 100; run++)
            {
                foreach (var builderType in _builderTypes)
                {
                    foreach (var clampDirection in Directions.Cardinals)
                    {
                        // Initialize
                        var didNotMoveOtherDirections = new Dictionary<IntVector2, int>();

                        var currentBuilderToTest = Activator.CreateInstance(builderType, _testChunk) as SpaceBuilder;
                        var initialValue = currentBuilderToTest.MaximalValues[clampDirection];
                        var expectedValue = initialValue;

                        if (clampDirection == Directions.Up || clampDirection == Directions.Right)
                        {
                            expectedValue -= 2;
                        }
                        else
                        {
                            expectedValue += 2;
                        }

                        foreach (var shouldNotMoveDirection in Directions.Cardinals)
                        {
                            if (shouldNotMoveDirection != clampDirection && shouldNotMoveDirection != -clampDirection)
                            {
                                didNotMoveOtherDirections[shouldNotMoveDirection] = currentBuilderToTest.MaximalValues[shouldNotMoveDirection];
                            }
                        }

                        // Assert No-Op Clamp
                        currentBuilderToTest.Clamp(clampDirection, initialValue);
                        var actualValue = currentBuilderToTest.MaximalValues[clampDirection];
                        Assert.AreEqual(actualValue, initialValue, $"Builder Type {builderType.Name} should not have clamped direction {clampDirection}, but clamped from {initialValue} to {actualValue}.");

                        // Assert Real Clamp
                        currentBuilderToTest.Clamp(clampDirection, expectedValue);
                        actualValue = currentBuilderToTest.MaximalValues[clampDirection];
                        Assert.AreEqual(actualValue, expectedValue, $"Builder Type {builderType.Name} should have clamped direction {clampDirection} to {expectedValue} but clamped from {initialValue} to {actualValue}.");

                        // Assert No Side Effects
                        foreach (var kvp in didNotMoveOtherDirections)
                        {
                            var actual = currentBuilderToTest.MaximalValues[kvp.Key];
                            Assert.AreEqual(actual, kvp.Value, $"Builder Type {builderType.Name} clamped direction {clampDirection} but moved in direction {kvp.Key} from {kvp.Value} to {actual}).");
                        }
                    }

                    // Assert Invalid Arguments Are Exceptional
                    foreach (var invalidClampDirection in Directions.Ordinals)
                    {
                        var currentBuilderToTest = Activator.CreateInstance(builderType, _testChunk) as SpaceBuilder;
                        Assert.Throws<ArgumentException>(() => currentBuilderToTest.Clamp(invalidClampDirection, 0));
                    }
                }
            }
        }

        [Test]
        public void SquashTest()
        {
            for (var run = 0; run < 100; run++)
            {
                Chance.Seed(run);
                foreach (var builderType in _builderTypes)
                {
                    foreach (var squashDirection in Directions.Cardinals)
                    {
                        // Initialize
                        var currentBuilderToTest = Activator.CreateInstance(builderType, _testChunk) as SpaceBuilder;
                        var initialValue = currentBuilderToTest.MaximalValues[squashDirection];
                        var expectedValue = initialValue;

                        if (squashDirection == Directions.Up || squashDirection == Directions.Right)
                        {
                            expectedValue -= 2;
                        }
                        else
                        {
                            expectedValue += 2;
                        }

                        var oppositeDirectionLimit = currentBuilderToTest.MaximalValues[-squashDirection];
                        currentBuilderToTest.AddBoundary(-squashDirection, oppositeDirectionLimit);

                        // Assert No-Op Squash
                        currentBuilderToTest.AddBoundary(squashDirection, initialValue);
                        var actualValue = currentBuilderToTest.MaximalValues[squashDirection];
                        Assert.AreEqual(actualValue, initialValue, $"Builder Type {builderType.Name} should not have squash direction {squashDirection}, but squash from {initialValue} to {actualValue} during run {run}.");

                        currentBuilderToTest.AddBoundary(squashDirection, expectedValue);
                        // Invalid spaces can't keep any previous promises and shouldn't be tested further
                        if (currentBuilderToTest.IsValid)
                        {
                            // Assert Actual Squash
                            actualValue = currentBuilderToTest.MaximalValues[squashDirection];
                            var distanceFromTarget = currentBuilderToTest.DistanceFrom(squashDirection, expectedValue);
                            Assert.LessOrEqual(distanceFromTarget, 0, $"Builder Type {builderType.Name} should have squash direction {squashDirection} to {expectedValue} but squash from {initialValue} to {actualValue} during run {run}.");

                            // Assert Did Not Shift Opposite Direction
                            var distanceFromPreviousMax = currentBuilderToTest.DistanceFrom(-squashDirection, oppositeDirectionLimit);
                            Assert.LessOrEqual(distanceFromPreviousMax, 0, $"Builder Type {builderType.Name} squash direction {squashDirection} but expanded in direction {-squashDirection} from {oppositeDirectionLimit} by {distanceFromPreviousMax} during run {run}.");
                        }
                    }

                    // Assert Invalid Arguments Are Exceptional
                    foreach (var invalidSquashDirection in Directions.Ordinals)
                    {
                        var currentBuilderToTest = Activator.CreateInstance(builderType, _testChunk) as SpaceBuilder;
                        Assert.Throws<ArgumentException>(() => currentBuilderToTest.Clamp(invalidSquashDirection, 0));
                    }
                }
            }
        }
    }
}
