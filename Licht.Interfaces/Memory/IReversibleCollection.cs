namespace Licht.Interfaces.Memory
{
    public interface IReversibleCollection<out T> : IEnumerableCollection<T>
    {
        T Previous { get; }
    }
}
