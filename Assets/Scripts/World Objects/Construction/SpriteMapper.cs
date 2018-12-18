using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Utilities.Debug;
using WorldObjects.Blocks;

namespace WorldObjects.Construction
{
    [Serializable]
    public class SpriteMapper
    {
        private readonly BlockTypes _type;
        private readonly Texture2D _source;

        private readonly Dictionary<IntVector2, Sprite> _cache = new Dictionary<IntVector2, Sprite>();

        public SpriteMapper(BlockTypes type, Texture2D source)
        {
            _type = type;
            _source = source;
        }

        public Sprite Fetch(IntVector2 worldPosition)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            if (!_cache.TryGetValue(worldPosition, out var sprite))
            {
                var pixelPositionX = (worldPosition.X * TEXTURE_SCALE) % _source.width;
                var pixelPositionY = (worldPosition.Y * TEXTURE_SCALE) % _source.height;

                var pixelPosition = new Vector2(pixelPositionX, pixelPositionY);

                sprite = Sprite.Create(_source, new Rect(pixelPosition, SPRITE_SIZE), SPRITE_ANCHOR, TEXTURE_SCALE);
                _cache.Add(worldPosition, sprite);
            }

            stopwatch.Stop();
            var log = new Log("TEST");
            log.Warning(stopwatch.Elapsed.ToString());

            return sprite;
        }

        private const int TEXTURE_SCALE = 64;
        private static readonly Vector2 SPRITE_SIZE = new Vector2(TEXTURE_SCALE, TEXTURE_SCALE);
        private static readonly Vector2 SPRITE_ANCHOR = new Vector2(0.5f, 0.5f);
    }
}