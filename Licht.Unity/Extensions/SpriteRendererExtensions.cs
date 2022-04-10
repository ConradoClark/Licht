using System;
using System.Collections.Generic;
using System.Text;
using Licht.Unity.Accessors;
using UnityEngine;

namespace Licht.Unity.Extensions
{
    public static class SpriteRendererExtensions
    {
        public static SpriteRendererAccessor GetAccessor(this SpriteRenderer spriteRenderer)
        {
            return new SpriteRendererAccessor(spriteRenderer);
        }
    }
}
