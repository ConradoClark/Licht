using System;
using System.Collections.Generic;
using System.Text;
using Licht.Impl.Globals;
using UnityEngine;

namespace Licht.Unity.Extensions
{
    public static class VectorExtensions
    {
        public static Vector2 RoundToPrecision(this Vector2 value, int precision)
        {
            return new Vector2(value.x.RoundToPrecision(precision),
                value.y.RoundToPrecision(precision));
        }
    }
}
