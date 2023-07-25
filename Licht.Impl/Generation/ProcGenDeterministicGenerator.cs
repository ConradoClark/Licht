namespace Licht.Impl.Generation
{
    public abstract class ProcGenDeterministicGenerator<TPosition>
    {
        private readonly ProcGenGenerator _generator;

        protected ProcGenDeterministicGenerator(int seed)
        {
            _generator = new ProcGenGenerator();
        }

        protected abstract int GenerateSeed(TPosition position);

        public float Generate(TPosition position, int seed)
        {
            var positionalSeed = GenerateSeed(position);
            return _generator.GenerateSeeded(PseudoRng.JenkinsHash(seed, positionalSeed));
        }
    }
}