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
                        Assert.AreEqual(actualValue, initialValue, $"Builder Type {builderType.Name} should have not have clamped direction {clampDirection}, but clamped from {initialValue} to {actualValue}.");

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
        public void CutTest()
        {
            for (var run = 0; run < 100; run++)
            {
                Chance.Seed(run);
                foreach (var builderType in _builderTypes)
                {
                    foreach (var cutDirection in Directions.Cardinals)
                    {
                        // Initialize
                        var didNotExpandOtherDirections = new Dictionary<IntVector2, int>();

                        var currentBuilderToTest = Activator.CreateInstance(builderType, _testChunk) as SpaceBuilder;
                        var initialValue = currentBuilderToTest.MaximalValues[cutDirection];
                        var expectedValue = initialValue;

                        if (cutDirection == Directions.Up || cutDirection == Directions.Right)
                        {
                            expectedValue -= 2;
                        }
                        else
                        {
                            expectedValue += 2;
                        }

                        foreach (var shouldNotExpandDirection in Directions.Cardinals)
                        {
                            if (shouldNotExpandDirection != cutDirection)
                            {
                                // Add a boundary in every other direction
                                didNotExpandOtherDirections[shouldNotExpandDirection] = currentBuilderToTest.MaximalValues[shouldNotExpandDirection];
                                currentBuilderToTest.AddBoundary(shouldNotExpandDirection, currentBuilderToTest.MaximalValues[shouldNotExpandDirection]);
                            }
                        }

                        // Assert No-Op Cut
                        currentBuilderToTest.AddBoundary(cutDirection, initialValue);
                        var actualValue = currentBuilderToTest.MaximalValues[cutDirection];
                        Assert.AreEqual(actualValue, initialValue, $"Builder Type {builderType.Name} should have not have cut direction {cutDirection}, but cut from {initialValue} to {actualValue} during run {run}.");

                        currentBuilderToTest.AddBoundary(cutDirection, expectedValue);
                        // Invalid spaces can't keep any previous promises and shouldn't be tested further
                        if (currentBuilderToTest.IsValid)
                        {
                            // Assert Actual Cut
                            actualValue = currentBuilderToTest.MaximalValues[cutDirection];
                            var distanceFromTarget = currentBuilderToTest.DistanceFrom(cutDirection, expectedValue);
                            Assert.LessOrEqual(distanceFromTarget, 0, $"Builder Type {builderType.Name} should have cut direction {cutDirection} to {expectedValue} but cut from {initialValue} to {actualValue} during run {run}.");

                            // Assert No Side Effects if the space is still valid
                            foreach (var kvp in didNotExpandOtherDirections)
                            {
                                var distanceFromPreviousMax = currentBuilderToTest.DistanceFrom(kvp.Key, kvp.Value);
                                Assert.LessOrEqual(distanceFromPreviousMax, 0, $"Builder Type {builderType.Name} cut direction {cutDirection} but expanded in direction {kvp.Key} from {kvp.Value} by {distanceFromPreviousMax} during run {run}.");
                            }
                        }
                    }

                    // Assert Invalid Arguments Are Exceptional
                    foreach (var invalidCutDirection in Directions.Ordinals)
                    {
                        var currentBuilderToTest = Activator.CreateInstance(builderType, _testChunk) as SpaceBuilder;
                        Assert.Throws<ArgumentException>(() => currentBuilderToTest.Clamp(invalidCutDirection, 0));
                    }
                }
            }
        }
    }
}
