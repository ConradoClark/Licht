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

        public ObjectPool(int size, IPoolableObjectFactory<T> objectFactory)
        {
            PoolSize = size;
            _objectFactory = objectFactory;
        }

        public bool Release(T obj)
        {
            if (!IsActive) return false;
            if (!_objectPool.Contains(obj)) return false;

            obj.Deactivate();
            return true;
        }

        public bool ReleaseAll()
        {
            if (!IsActive) return false;
            foreach (var obj in _objectPool)
            {
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

        public bool TryGetFromPool(out T obj)
        {
            if (!IsActive)
            {
                obj = default(T);
                return false;
            }

            var available = _objectPool.Where(o => !o.IsActive);
            var enumerable = available as T[] ?? available.ToArray();

            if (enumerable.Any())
            {
                obj = enumerable.First();
                obj.Activate();
                return true;
            }
            obj = default(T);
            return false;
        }

        public bool GetManyFromPool(int amount, out T[] objects)
        {
            if (amount <= 0 || !IsActive)
            {
                objects = new T[0];
                return false;
            }

            var available = _objectPool.Where(o => !o.IsActive).Take(amount);
            var enumerable = available as T[] ?? available.ToArray();
            foreach(T obj in enumerable)
            {
                obj.Activate();
            }
            if (enumerable.Skip(amount-1).Any())
            {
                objects = enumerable.ToArray();
                return true;
            }
            objects = enumerable.ToArray();
            return false;
        }
    }
}