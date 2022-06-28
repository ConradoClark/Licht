using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Interfaces.Generation;

namespace Licht.Impl.Generation
{
    public class WeightedDice<TValue> : IGenerator<int, TValue> where TValue : IWeighted<float>
    {
        private readonly IReadOnlyList<TValue> _values;
        private readonly IGenerator<int, float> _floatGenerator;
        private readonly List<TValue> _pickedValues;
        private readonly bool _repeatValues;
        public WeightedDice(IEnumerable<TValue> possibleValues, IGenerator<int, float> randomFloatGenerator, bool repeatValues = true)
        {
            _values = possibleValues.ToArray();
            _floatGenerator = randomFloatGenerator;
            _floatGenerator.Seed = Seed;
            _pickedValues = new List<TValue>();
            _repeatValues = repeatValues;
        }
        public int Seed { get; set; }

        public TValue Generate()
        {
            var listToUse = _repeatValues ? _values : _values.Except(_pickedValues).ToArray();

            if (listToUse.Count == 0) return default;

            var sumOfWeights = listToUse.Sum(v => v.Weight);
            var normalizedWeights = listToUse.Aggregate(Array.Empty<float>(),
                (acc, next) => acc.Length == 0 ? new[] { next.Weight / sumOfWeights }
                    : acc.Concat(new[] { acc.Last() + next.Weight / sumOfWeights }).ToArray());

            var result = _floatGenerator.Generate();

            for (var i = 0; i < normalizedWeights.Length; i++)
            {
                if (!(result <= normalizedWeights[i])) continue;

                _pickedValues.Add(listToUse[i]);
                return listToUse[i];
            }

            throw new Exception("This should never happen: value not found amongst weights.");
        }
    }
}
