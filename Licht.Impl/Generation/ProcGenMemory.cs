using System.Collections.Generic;
using Licht.Interfaces.Update;

namespace Licht.Impl.Generation
{
    public class ProcGenMemory<TPosition> : ICanInitialize
    {
        private Dictionary<(TPosition, int), ProcGenParameters> _memoryDict;
        private HashSet<(TPosition, int)> _destroyedSet;

        public ProcGenMemory()
        {
            Initialize();
        }

        public void Initialize()
        {
            _memoryDict ??= new Dictionary<(TPosition, int), ProcGenParameters>();
            _destroyedSet ??= new HashSet<(TPosition, int)>();
        }

        public void SaveMemory(TPosition position, int layer, ProcGenParameters parameters)
        {
            _memoryDict[(position, layer)] = MergeParameters(position, layer, parameters);
        }

        public bool TryGetMemory(TPosition position, int layer, out ProcGenParameters parameters)
        {
            return _memoryDict.TryGetValue((position, layer), out parameters);
        }

        public bool IsDestroyed(TPosition position, int layer)
        {
            return _destroyedSet.Contains((position, layer));
        }

        public void SetDestroyed(TPosition position, int layer, bool destroyed)
        {
            switch (destroyed)
            {
                case true when !_destroyedSet.Contains((position, layer)):
                    _destroyedSet.Add((position, layer));
                    return;
                case false when _destroyedSet.Contains((position, layer)):
                    _destroyedSet.Remove((position, layer));
                    break;
            }
        }

        private ProcGenParameters MergeParameters(TPosition position, int layer, ProcGenParameters parameters)
        {
            if (!_memoryDict.TryGetValue((position, layer), out var original)) return parameters;

            if (parameters.Booleans != null)
            {
                original.Booleans ??= new Dictionary<string, bool>();
                foreach (var param in parameters.Booleans)
                {
                    original.Booleans[param.Key] = param.Value;
                }
            }
        
            if (parameters.Ints != null)
            {
                original.Ints ??= new Dictionary<string, int>();
                foreach (var param in parameters.Ints)
                {
                    original.Ints[param.Key] = param.Value;
                }
            }
        
            if (parameters.Floats != null)
            {
                original.Floats ??= new Dictionary<string, float>();
                foreach (var param in parameters.Floats)
                {
                    original.Floats[param.Key] = param.Value;
                }
            }
        
            if (parameters.Strings != null)
            {
                original.Strings ??= new Dictionary<string, string>();
                foreach (var param in parameters.Strings)
                {
                    original.Strings[param.Key] = param.Value;
                }
            }

            return original;
        }
    }
}