using System;

namespace Licht.Impl.Globals
{
    public static class MathExtensions
    {
        public static float RoundToPrecision(this float value, int precision)
        {
            var multiplier = MathF.Pow(10f, precision);
            var roundedValue = MathF.Round(value * multiplier) / multiplier;
            return roundedValue;
        }
    }
}
