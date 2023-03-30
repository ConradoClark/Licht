using Licht.Interfaces.Update;

namespace Licht.Interfaces.Time
{
    public interface ITimer: IUpdateable, IResettable, ICanActivate, ICanDeactivate
    {
        float Multiplier { get; set; }
        float TotalElapsedTimeInMilliseconds { get; }
        float UpdatedTimeInMilliseconds { get; }
        bool Set(float time);
    }
}