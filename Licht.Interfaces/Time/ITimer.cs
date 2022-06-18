using Licht.Interfaces.Update;

namespace Licht.Interfaces.Time
{
    public interface ITimer: IUpdateable, IResettable, IActivable, IDeactivable
    {
        double Multiplier { get; set; }
        double TotalElapsedTimeInMilliseconds { get; }
        double UpdatedTimeInMilliseconds { get; }
        bool Set(double time);
    }
}