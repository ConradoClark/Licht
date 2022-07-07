namespace Licht.Interfaces.Generation
{
    public interface IWeighted<out TValue>
    {
        TValue Weight { get; }
    }
}
