using Licht.Interfaces.Time;

namespace Licht.Unity.Objects
{
    public abstract class ScriptTimer : ScriptValue
    {
        public abstract ITimer Timer { get; }
    }
}
