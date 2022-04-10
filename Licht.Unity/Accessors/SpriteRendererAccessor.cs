using System;
using System.Collections.Generic;
using System.Text;
using Licht.Unity.Builders;
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
    }
}
