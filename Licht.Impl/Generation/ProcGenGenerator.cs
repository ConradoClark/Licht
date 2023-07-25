using System;
using Licht.Interfaces.Generation;

namespace Licht.Impl.Generation
{
    public class ProcGenGenerator: IGenerator<int, float>
    {
        public int Seed { get; set; }

        private readonly PseudoRng _baseRandomizer;

        public ProcGenGenerator(int? seed = null)
        {
            seed ??= Environment.TickCount;
            Seed = (int) seed;
            _baseRandomizer = new PseudoRng((uint) Seed);
        }
        
        public float Generate()
        {
            return _baseRandomizer.NextSingle();
        }

        public float GenerateSeeded(int seed)
        {
            Reseed(seed);
            return Generate();
        }

        private void Reseed(int seed)
        {
            _baseRandomizer.Reseed( (uint) (Seed ^ seed));
        }
    }
}