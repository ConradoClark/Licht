﻿using Licht.Interfaces.Update;

namespace Licht.Interfaces.Pooling
{
    public interface IPool : IActivable
    {
        int AvailableObjects { get; }
        int ObjectsInUse { get; }
        int PoolSize { get; }
        bool Release(IPoolableObject obj);

        bool ReleaseAll();
    }

    public interface IPool<T> : IPool where T : IPoolableObject
    {
        bool TryGetFromPool(out T obj);
        bool GetManyFromPool(int amount, out T[] objects);
    }
}