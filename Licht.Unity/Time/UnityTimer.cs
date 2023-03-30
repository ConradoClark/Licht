using Licht.Impl.Debug;
using Licht.Impl.Memory;
using Licht.Interfaces.Time;

namespace Licht.Unity.Time
{
    public class UnityTimer : ITimer
    {
        public UnityTimer()
        {
            _last = new Caterpillar<float> { TailSize = 1 };
        }

        private float _offset;
        private readonly Caterpillar<float> _last;
        private float _elapsed;

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

            var latest = UnityEngine.Time.deltaTime * Multiplier * 1000f;
            _elapsed += latest;
            _last.Current = latest;

            if (Debug)
            {
                DebugLicht.Write($"{latest} ms");
            }
        }
    }
}