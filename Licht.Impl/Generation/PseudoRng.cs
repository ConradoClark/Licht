using System;

namespace Licht.Impl.Generation
{
    [Serializable]
    public class PseudoRng
    {
        private const int N = 624;
        private const int M = 397;
        private const uint MatrixA = 0x9908b0df;
        private const uint UpperMask = 0x80000000;
        private const uint LowerMask = 0x7fffffff;

        private uint[] _mersenneTwister = new uint[N];
        private int _twisterIndex = N + 1;
        public uint CurrentSeed { get; private set; }

        public PseudoRng() : this((uint)Environment.TickCount)
        {
        }

        public PseudoRng(uint seed)
        {
            Reseed(seed);
        }

        public void Reseed(int x, int y)
        {
            var seed = JenkinsHash(x, y);
            Reseed((uint)seed);
        }

        public void Reseed(uint seed)
        {
            CurrentSeed = seed;
            _mersenneTwister[0] = seed & 0xffffffff;
            for (_twisterIndex = 1; _twisterIndex < N; _twisterIndex++)
            {
                _mersenneTwister[_twisterIndex] =
                    (uint)(1812433253 * (_mersenneTwister[_twisterIndex - 1] ^
                                         (_mersenneTwister[_twisterIndex - 1] >> 30)) + _twisterIndex);
                _mersenneTwister[_twisterIndex] &= 0xffffffff;
            }
        }
        
        public void Reset()
        {
            Reseed(CurrentSeed);
        }

        public static int JenkinsHash(int a, int b)
        {
            unchecked
            {
                a = a + 0x7ed55d16 + (a << 12);
                a = a ^ (int)0xc761c23c ^ (a >> 19);
                a = a + 0x165667b1 + (a << 5);
                a = (a + (int)0xd3a2646c) ^ (a << 9);
                a = a + (int)0xfd7046c5 + (a << 3);
                a = a ^ (int)0xb55a4f09 ^ (a >> 16);

                b = b + 0x7ed55d16 + (b << 12);
                b = b ^ (int)0xc761c23c ^ (b >> 19);
                b = b + 0x165667b1 + (b << 5);
                b = (b + (int)0xd3a2646c) ^ (b << 9);
                b = b + (int)0xfd7046c5 + (b << 3);
                b = b ^ (int)0xb55a4f09 ^ (b >> 16);

                return a ^ b;
            }
        }

        public uint GenerateUInt()
        {
            uint y;
            uint[] mag01 = { 0x0, MatrixA };
            if (_twisterIndex >= N)
            {
                int it;
                for (it = 0; it < N - M; it++)
                {
                    y = (_mersenneTwister[it] & UpperMask) | (_mersenneTwister[it + 1] & LowerMask);
                    _mersenneTwister[it] = _mersenneTwister[it + M] ^ (y >> 1) ^ mag01[y & 0x1];
                }

                for (; it < N - 1; it++)
                {
                    y = (_mersenneTwister[it] & UpperMask) | (_mersenneTwister[it + 1] & LowerMask);
                    _mersenneTwister[it] = _mersenneTwister[it + (M - N)] ^ (y >> 1) ^ mag01[y & 0x1];
                }

                y = (_mersenneTwister[N - 1] & UpperMask) | (_mersenneTwister[0] & LowerMask);
                _mersenneTwister[N - 1] = _mersenneTwister[M - 1] ^ (y >> 1) ^ mag01[y & 0x1];
                _twisterIndex = 0;
            }

            y = _mersenneTwister[_twisterIndex++];
            y ^= y >> 11;
            y ^= (y << 7) & 0x9d2c5680;
            y ^= (y << 15) & 0xefc60000;
            y ^= y >> 18;
            return y;
        }

        public int Next()
        {
            return (int)(GenerateUInt() >> 1);
        }

        public int Next(int maxValue)
        {
            return Next(0, maxValue);
        }

        public int Next(int minValue, int maxValue)
        {
            if (maxValue < minValue)
            {
                throw new ArgumentOutOfRangeException(nameof(minValue), "minValue cannot be less than maxValue.");
            }

            return minValue + (int)((maxValue - minValue) * Sample());
        }

        public float NextSingle()
        {
            return (float)GenerateUInt() / uint.MaxValue;
        }

        public void NextBytes(byte[] buffer)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            for (var i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte)(GenerateUInt() % (byte.MaxValue + 1));
            }
        }

        private double Sample()
        {
            return (double)GenerateUInt() / uint.MaxValue;
        }
    }
}