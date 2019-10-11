using Licht.Interfaces.Update;
using System.Collections.Generic;

namespace Licht.Interfaces.Pooling
{
    public interface IPool : IActivable
    {
        int AvailableObjects { get; }
        int ObjectsInUse { get; }
        int PoolSize { get; }
    }

    public interface IPool<T> : IPool where T : IPoolableObject
    {
        bool TryGetFromPool(out T obj);
        bool GetManyFromPool(int amount, out T[] objects);
        bool Release(T obj);
    }
}