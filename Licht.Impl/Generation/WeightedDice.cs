using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Debug;
using Licht.Interfaces.Generation;

namespace Licht.Impl.Generation
{
    public class WeightedDice<TValue> : IGenerator<int, TValue> where TValue : IWeighted<float>
    {
        private readonly IReadOnlyList<TValue> _values;
        private readonly IGenerator<int, float> _floatGenerator;
        public WeightedDice(IEnumerable<TValue> possibleValues, IGenerator<int, float> randomFloatGenerator)
        {
            _values = possibleValues.ToArray();
            _floatGenerator = randomFloatGenerator;
            _floatGenerator.Seed = Seed;
        }
        public int Seed { get; set; }
        public TValue Generate()
        {
            if (_values.Count == 0) return default;

            var sumOfWeights = _values.Sum(v => v.Weight);
            var normalizedWeights = _values.Aggregate(new float[0],
                (acc, next) => acc.Length == 0 ? new[] { next.Weight / sumOfWeights }
                    : acc.Concat(new[] { acc.Last() + next.Weight / sumOfWeights }).ToArray());

            var result = _floatGenerator.Generate();

            for (var i = 0; i < normalizedWeights.Length; i++)
            {
                if (result <= normalizedWeights[i]) return _values[i];
            }

            throw new Exception("This should never happen: value not found amongst weights.");
        }
    }
}
