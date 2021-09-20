using System;
using JetBrains.Annotations;

namespace Licht.Impl.Debug
{
    [PublicAPI]
    public static class DebugLicht
    {
        public static Action<string> Write { get; set; } = Console.WriteLine;
    }
}
