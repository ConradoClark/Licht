using Licht.Impl.Time;
using Licht.Interfaces.Time;
using UnityEngine;

namespace Licht.Unity.Objects
{
    [CreateAssetMenu(fileName = "DefaultTimer", menuName = "Licht/Timers/DefaultTimer", order = 1)]
    public class DefaultTimerScriptable : TimerScriptable
    {
        public override object Value => Timer;
        public override ITime Timer { get; } = new DefaultTimer(() => Time.deltaTime * 1000);
    
}
}