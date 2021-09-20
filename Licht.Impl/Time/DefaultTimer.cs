using Licht.Interfaces.Time;
using System;
using JetBrains.Annotations;
using Licht.Impl.Debug;
using Licht.Impl.Memory;

namespace Licht.Impl.Time
{
    [PublicAPI]
    public class DefaultTimer : ITime
    {
        private readonly Func<double> _timeStepFn;
        public DefaultTimer(Func<double> timeStepFn, int framesPerSecond = 60)
        {
            _last = new Caterpillar<double> { TailSize = 1 };
            _timeStepFn = timeStepFn;
            FramesPerSecond = framesPerSecond;
        }

        private double _offset;
        private readonly Caterpillar<double> _last;
        private double _elapsed;

        public int FramesPerSecond { get; set; }
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

        public bool Reset()
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

            var latest = _timeStepFn() * Multiplier;
            var frameDuration = 1d / FramesPerSecond * 1000d;

            var difference = frameDuration - latest;

            latest = Math.Min(frameDuration, latest - latest * difference / frameDuration);

            _elapsed += latest;
            _last.Current = latest;

            if (Debug)
            {
                DebugLicht.Write($"{latest} ms");
            }
        }
    }
}