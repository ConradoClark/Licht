using System;

namespace Licht.Impl.Debug
{
    public static class DebugLicht
    {
        public static Action<string> Write { get; set; } = Console.WriteLine;
    }
}
