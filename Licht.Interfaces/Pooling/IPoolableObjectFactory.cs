namespace Licht.Interfaces.Pooling
{
    public interface IPoolableObjectFactory<out T> where T: IPoolableObject
    {
        T Instantiate();
    }
}
