using System;
using JetBrains.Annotations;

namespace Licht.Impl.Globals
{
    [PublicAPI]
    public static class NumericGlobals
    {
        public static float FloatTolerance = 0.001f;

        public static bool FloatEq(this float number, float comparison)
        {
            return Math.Abs(number - comparison) < FloatTolerance;
        }
    }
}
