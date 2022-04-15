using Licht.Interfaces.Time;

namespace Licht.Unity.Objects
{
    public abstract class TimerScriptable : ScriptableValue
    {
        public abstract ITime Timer { get; }
    }
}
