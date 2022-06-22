using System;
using Licht.Impl.Debug;
using Licht.Impl.Memory;
using Licht.Interfaces.Time;

namespace Licht.Unity.Time
{
    public class UnityTimer : ITimer
    {
        public UnityTimer()
        {
            _last = new Caterpillar<double> { TailSize = 1 };
        }

        private double _offset;
        private readonly Caterpillar<double> _last;
        private double _elapsed;

        public double Multiplier { get; set; } = 1d;
        public double TotalElapsedTimeInMilliseconds => _elapsed + _offset;
        public double UpdatedTimeInMilliseconds => _last.Current;
        public bool IsActive { get; private set; } = true;
        public bool Debug { get; set; }

        public bool Activate()
        {
            IsActive = true;
            return true;
        }

        public bool Deactivate()
        {
            IsActive = false;
            return true;
        }

        public bool PerformReset()
        {
            if (!IsActive) return true;

            _offset = 0d;
            _elapsed = 0d;

            return true;
        }

        public bool Set(double time)
        {
            _offset = time;
            _elapsed = 0d;

            return true;
        }

        public void Update()
        {
            if (!IsActive) return;

            var latest = UnityEngine.Time.deltaTime * Multiplier * 1000d;
            _elapsed += latest;
            _last.Current = latest;

            if (Debug)
            {
                DebugLicht.Write($"{latest} ms");
            }
        }
    }
}