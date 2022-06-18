using UnityEngine;

namespace Licht.Unity.Accessors
{
    public class SpriteRendererAccessor
    {
        private readonly SpriteRenderer _spriteRenderer;

        public SpriteRendererAccessor(SpriteRenderer spriteRenderer)
        {
            _spriteRenderer = spriteRenderer;
        }

        public ColorAccessor Color => new ColorAccessor(value => _spriteRenderer.color = value,
            () => _spriteRenderer.color);

        public MaterialAccessor Material(string property)
        {
            return new MaterialAccessor(property, _spriteRenderer);
        }
    }
}
