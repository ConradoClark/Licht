using UnityEngine;

namespace Licht.Unity.Extensions
{
    public static class LayerMaskExtensions
    {
        public static bool Contains(this LayerMask source, int layer)
        {
            return source == (source | (1 << layer));
        }
    }
}
