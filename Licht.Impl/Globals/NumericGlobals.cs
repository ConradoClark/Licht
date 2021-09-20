using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
