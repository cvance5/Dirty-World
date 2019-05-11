using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using WorldObjects.Spaces;
using WorldObjects.WorldGeneration;

namespace Tests.SpaceTests
{
    public class TunnelTest
    {
        private static ChunkBuilder _testChunk;

        [SetUp]
        public void ClearScene() => EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
        [SetUp]
        public void PrepareTestChunk() => _testChunk = new ChunkBuilder(Vector2.zero, 32);

        [Test]
        public void TunnelContainsTest()
        {
            var tunnel = new Tunnel(new IntVector2(-2, -2), new IntVector2(2, 2));

            for (var x = -5; x <= 5; x++)
            {
                for (var y = -5; y <= 5; y++)
                {
                    var expectation = y >= -2 &&
                                      y <= 2 &&
                                      x >= -2
                                      && x <= 2;

                    var actual = tunnel.Extents.Contains(new IntVector2(x, y));

                    Assert.AreEqual(actual, expectation, $"Position: [{x},{y}] | Expected: {expectation} | Actual: {actual}");
                }
            }
        }
    }
}
