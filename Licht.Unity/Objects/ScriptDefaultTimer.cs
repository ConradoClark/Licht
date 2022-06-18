using System;
using Licht.Impl.Time;
using Licht.Interfaces.Time;
using UnityEngine;

namespace Licht.Unity.Objects
{
    [CreateAssetMenu(fileName = "DefaultTimer", menuName = "Licht/Timers/DefaultTimer", order = 1)]
    public class ScriptDefaultTimer : ScriptTimer
    {
        private double _setMultiplier = 1;
        public double Multiplier = 1;
        public override object Value => Timer;

        private ITimer _timer;

        public override ITimer Timer
        {
            get
            {
                var timer = _timer ??= new DefaultTimer(() => Time.deltaTime * 1000f)
                {
                    Multiplier = Multiplier
                };

                if (Math.Abs(Multiplier - _setMultiplier) > 0.01f)
                {
                    timer.Multiplier = _setMultiplier = Multiplier;
                }

                return timer;
            }
        }
    }
}