using Licht.Interfaces.Update;

namespace Licht.Interfaces.Time
{
    public interface ITime: IUpdateable, IResettable, IActivable, IDeactivable
    {
        double Multiplier { get; }
        double TotalElapsedTimeInMilliseconds { get; }
        double UpdatedTimeInMilliseconds { get; }
        bool Set(double time);
    }
}