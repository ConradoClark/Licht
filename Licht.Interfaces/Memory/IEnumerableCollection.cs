namespace Licht.Interfaces.Memory
{
    public interface IEnumerableCollection<out T>
    {
        T Current { get; }
        T Next { get; }
    }
}
