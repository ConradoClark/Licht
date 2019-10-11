﻿using System;

namespace Licht.Impl.Globals
{
    public static class NumericGlobals
    {
        public static float FloatTolerance = 0.001f;

        public static bool FloatEq(this float number, float comparison)
        {
            return Math.Abs(number - comparison) < FloatTolerance;
        }
    }
}
