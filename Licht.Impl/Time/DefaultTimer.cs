using Licht.Interfaces.Time;
using System;
using JetBrains.Annotations;
using Licht.Impl.Debug;
using Licht.Impl.Memory;

namespace Licht.Impl.Time
{
    [PublicAPI]
    public class DefaultTimer : ITimer
    {
        private readonly Func<float> _timeStepFn;
        public DefaultTimer(Func<float> timeStepFn, int framesPerSecond = 60)
        {
            _last = new Caterpillar<float> { TailSize = 1 };
            _timeStepFn = timeStepFn;
            FramesPerSecond = framesPerSecond;
        }

        private float _offset;
        private readonly Caterpillar<float> _last;
        private float _elapsed;

        public int FramesPerSecond { get; set; }
        public float Multiplier { get; set; } = 1f;
        public float TotalElapsedTimeInMilliseconds => _elapsed + _offset;
        public float UpdatedTimeInMilliseconds => _last.Current;
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

            _offset = 0f;
            _elapsed = 0f;

            return true;
        }

        public bool Set(float time)
        {
            _offset = time;
            _elapsed = 0f;

            return true;
        }

        public void Update()
        {
            if (!IsActive) return;

            var latest = _timeStepFn();
            var frameDuration = 1f / FramesPerSecond * 1000f;

            var difference = frameDuration - latest;

            latest = Math.Min(frameDuration, latest - latest * difference / frameDuration) * Multiplier;

            _elapsed += latest;
            _last.Current = latest;

            if (Debug)
            {
                DebugLicht.Write($"{latest} ms");
            }
        }
    }
}