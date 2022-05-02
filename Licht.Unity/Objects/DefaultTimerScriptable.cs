using System;
using Licht.Impl.Time;
using Licht.Interfaces.Time;
using UnityEngine;

namespace Licht.Unity.Objects
{
    [CreateAssetMenu(fileName = "DefaultTimer", menuName = "Licht/Timers/DefaultTimer", order = 1)]
    public class DefaultTimerScriptable : TimerScriptable
    {
        private double _setMultiplier = 1;
        public double Multiplier = 1;
        public override object Value => Timer;

        private ITime _timer;

        public override ITime Timer
        {
            get
            {
                var timer = _timer ?? (_timer = new DefaultTimer(() => Time.deltaTime * 1000f)
                {
                    Multiplier = Multiplier
                });

                if (Math.Abs(Multiplier - _setMultiplier) > 0.01f)
                {
                    timer.Multiplier = _setMultiplier = Multiplier;
                }

                return timer;
            }
        }
    }
}