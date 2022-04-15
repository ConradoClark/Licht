using Licht.Impl.Time;
using UnityEngine;

namespace Licht.Unity.Objects
{
    [CreateAssetMenu(fileName = "DefaultTimer", menuName = "Licht/Timers/DefaultTimer", order = 1)]
    public class DefaultTimerScriptable : ScriptableValue
    {
        public override object Value => Timer;
        public DefaultTimer Timer { get; } = new DefaultTimer(() => Time.deltaTime * 1000);
    
}
}