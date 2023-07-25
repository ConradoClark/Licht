using System.Collections.Generic;
using Licht.Interfaces.Update;

namespace Licht.Impl.Generation
{
    public class ProcGenParameters : ICanInitialize
    {
        public Dictionary<string, bool> Booleans;
        public Dictionary<string, int> Ints;
        public Dictionary<string, float> Floats;
        public Dictionary<string, string> Strings;

        public ProcGenParameters()
        {
            Initialize();
        }

        public void Initialize()
        {
            Booleans ??= new Dictionary<string, bool>();
            Ints ??= new Dictionary<string, int>();
            Floats ??= new Dictionary<string, float>();
            Strings ??= new Dictionary<string, string>();
        }
    }
}