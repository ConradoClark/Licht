using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Licht.Impl.Debug
{
    public static class DebugLicht
    {
        public static Action<string> Write { get; set; } = Console.WriteLine;
    }
}
