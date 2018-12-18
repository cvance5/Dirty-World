using System.Collections.Generic;
using UnityEngine;
using WorldObjects.Blocks;

namespace WorldObjects.Construction
{
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
            var pixelPositionX = (worldPosition.X * TEXTURE_SCALE) % _source.width;
            var pixelPositionY = (worldPosition.Y * TEXTURE_SCALE) % _source.height;

            var pixelPosition = new Vector2(pixelPositionX, pixelPositionY);

            if (!_cache.TryGetValue(pixelPosition, out var sprite))
            {
                sprite = Sprite.Create(_source, new Rect(pixelPosition, SPRITE_SIZE), SPRITE_ANCHOR, TEXTURE_SCALE);
                _cache.Add(pixelPosition, sprite);
            }

            return sprite;
        }

        private const int TEXTURE_SCALE = 64;
        private static readonly Vector2 SPRITE_SIZE = new Vector2(TEXTURE_SCALE, TEXTURE_SCALE);
        private static readonly Vector2 SPRITE_ANCHOR = new Vector2(0.5f, 0.5f);
    }
}