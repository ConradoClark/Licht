using System;
using Licht.Impl.Generation;
using Licht.Interfaces.Generation;
using UnityEngine;

namespace Licht.Unity.Generation
{
    public class Grid2DPositionalGenerator : ProcGenDeterministicGenerator<Vector2Int>, IGenerator<int,float>
    {
        public Vector2Int Position { get; set; }
        public Grid2DPositionalGenerator(int seed) : base(seed)
        {
            Seed = seed;
        }

        protected override int GenerateSeed(Vector2Int position)
        {
            var (int1, int2) = ConvertFloatTo32BitInt(position.x, position.y);
            return PseudoRng.JenkinsHash(int1, int2);
        }
        
        private static (int, int) ConvertFloatTo32BitInt(float float1, float float2)
        {
            return (
                BitConverter.ToInt32(BitConverter.GetBytes(float1), 0),
                BitConverter.ToInt32(BitConverter.GetBytes(float2), 0));
        }

        public int Seed { get; set; }
        
        public float Generate()
        {
            return Generate(Position, Seed);
        }
    }
}