using System;
using Licht.Interfaces.Pooling;
using System.Linq;
using JetBrains.Annotations;

namespace Licht.Impl.Pooling
{

    [PublicAPI]
    public class ObjectPool<T> : IPool<T> where T : IPoolableObject
    {
        private T[] _objectPool;
        public int AvailableObjects => !IsActive ? 0 : _objectPool.Count(o => !o.IsActive);
        public int ObjectsInUse => !IsActive ? 0 : _objectPool.Count(o => o.IsActive);
        public bool IsActive { get; private set; }
        public int PoolSize { get; }
        private readonly IPoolableObjectFactory<T> _objectFactory;

        private T[] _tempPool;

        public ObjectPool(int size, IPoolableObjectFactory<T> objectFactory)
        {
            PoolSize = size;
            _objectFactory = objectFactory;
            _tempPool = new T[50];
        }

        public T[] GetActiveObjects()
        {
            return _objectPool.Where(obj => obj.IsActive).ToArray();
        }

        public bool Release(IPoolableObject obj)
        {
            if (!IsActive) return false;
            if (_objectPool.All(o=> !Equals(o, obj))) return false;
            if (obj == null) return false;

            obj.Deactivate();
            return true;
        }

        public bool ReleaseAll()
        {
            if (!IsActive) return false;
            foreach (var obj in _objectPool)
            {
                if (obj == null) continue;
                obj.Deactivate();
            }

            return true;
        }

        public bool Activate()
        {
            if (IsActive) return false;
            _objectPool = new T[PoolSize];
            for (var i = 0; i < PoolSize; i++)
            {
                var obj = _objectFactory.Instantiate();
                if (obj == null) continue;
                _objectPool[i] = obj;
                _objectPool[i].Initialize();
            }
            IsActive = true;
            return true;
        }

        public bool TryGetFromPool(out T obj, Action<T> beforeActivation = null)
        {
            if (!IsActive)
            {
                obj = default;
                return false;
            }

            for (var index = 0; index < _objectPool.Length; index++)
            {
                if (_objectPool[index].IsActive) continue;
                obj = _objectPool[index];
                obj.Pool = this;

                beforeActivation?.Invoke(obj);
                obj.Activate();
                return true;
            }

            obj = default;
            return false;
        }

        public bool GetManyFromPool(int amount, out T[] objects, Action<T, int> beforeActivation = null)
        {
            if (amount <= 0 || !IsActive)
            {
                objects = Array.Empty<T>();
                return false;
            }

            for (var index = 0; index < _tempPool.Length; index++)
            {
                _tempPool[index] = default;
            }

            var currentIndex = 0;
            for (var index = 0; index < _objectPool.Length; index++)
            {
                var obj = _objectPool[index];
                obj.Pool = this;

                if (obj.IsActive) continue;

                beforeActivation?.Invoke(obj, index);
                obj.Activate();
                _tempPool[currentIndex] = obj;
                currentIndex++;

                if (currentIndex >= amount) break;
            }

            objects = new T[currentIndex];
            Array.Copy(_tempPool, objects, currentIndex);

            return currentIndex == amount;
        }
    }
}