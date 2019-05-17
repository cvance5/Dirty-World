using MathConcepts;
using NUnit.Framework;
using System.Diagnostics;
using UnityEngine;
using Utilities.Debug;
using WorldObjects.Construction;

namespace Tests.PerformanceTests
{
    public class SpriteMapperTest
    {
        private static readonly Texture2D SAMPLE_TEXTURE = new Texture2D(2048, 2048);
        private static readonly Vector2 SPRITE_SIZE = new Vector2(64, 64);
        private static readonly Vector2 SPRITE_ANCHOR = new Vector2(0.5f, 0.5f);

        [Test]
        public void NotSlowerUncachedTest()
        {
            var stopwatch = new Stopwatch();

            var spriteMapper = new SpriteMapper(WorldObjects.Blocks.BlockTypes.Dirt, SAMPLE_TEXTURE);

            stopwatch.Start();
            for (var x = -16; x < 16; x++)
            {
                for (var y = -16; y < 16; y++)
                {
                    var pixelPositionX = (x * 64) % SAMPLE_TEXTURE.width;
                    var pixelPositionY = (y * 64) % SAMPLE_TEXTURE.height;

                    var pixelPosition = new Vector2(pixelPositionX, pixelPositionY);

                    Sprite.Create(SAMPLE_TEXTURE, new Rect(pixelPosition, SPRITE_SIZE), SPRITE_ANCHOR, 64);
                }
            }
            stopwatch.Stop();

            var rawUncachedTime = stopwatch.Elapsed;

            stopwatch.Reset();

            stopwatch.Start();
            for (var x = -16; x < 16; x++)
            {
                for (var y = -16; y < 16; y++)
                {
                    spriteMapper.Fetch(new IntVector2(x, y));
                }
            }
            stopwatch.Stop();

            var mapperUncachedTime = stopwatch.Elapsed;

            Assert.LessOrEqual((mapperUncachedTime - rawUncachedTime).TotalSeconds, 3);            
        }

        [Test]
        public void FasterCached()
        {
            var stopwatch = new Stopwatch();
            var spriteMapper = new SpriteMapper(WorldObjects.Blocks.BlockTypes.Dirt, SAMPLE_TEXTURE);

            stopwatch.Start();
            for (var x = -160; x < 160; x++)
            {
                for (var y = -160; y < 160; y++)
                {
                    var pixelPositionX = (x * 64) % SAMPLE_TEXTURE.width;
                    var pixelPositionY = (y * 64) % SAMPLE_TEXTURE.height;

                    var pixelPosition = new Vector2(pixelPositionX, pixelPositionY);

                    Sprite.Create(SAMPLE_TEXTURE, new Rect(pixelPosition, SPRITE_SIZE), SPRITE_ANCHOR, 64);
                }
            }
            stopwatch.Stop();

            var uncachedTime = stopwatch.Elapsed;

            stopwatch.Reset();

            stopwatch.Start();
            for (var x = -160; x < 160; x++)
            {
                for (var y = -160; y < 160; y++)
                {
                    spriteMapper.Fetch(new IntVector2(x, y));
                }
            }
            stopwatch.Stop();

            var mapperCachedTime = stopwatch.Elapsed;

            Assert.LessOrEqual(mapperCachedTime, uncachedTime);

            _log.Info($"Raw Time : {uncachedTime}, Cached Time: {mapperCachedTime}.");
        }

        private static readonly Log _log = new Log("SpriteMapperTest");
    }
}
