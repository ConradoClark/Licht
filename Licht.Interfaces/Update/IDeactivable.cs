namespace Licht.Interfaces.Update
{
    public interface IDeactivable : IActivationReportable
    {
        bool Deactivate();
    }
}
