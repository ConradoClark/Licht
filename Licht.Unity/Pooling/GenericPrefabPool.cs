using System;
using Licht.Impl.Pooling;
using Licht.Interfaces.Pooling;
using Licht.Interfaces.Update;
using UnityEngine;

namespace Licht.Unity.Pooling
{
    [DefaultExecutionOrder(-1000)]
    public class GenericPrefabPool<T> : MonoBehaviour, IPoolableObjectFactory<T>, IActivable
        where T: IPoolableComponent
    {
        public bool AutoActivate;
        public GameObject Prefab;
        public int Size;
        private ObjectPool<T> _objectPool;
        private int _index;

        public bool IsActive { get; protected set; }

        private void OnEnable()
        {
            if (!AutoActivate || Prefab == null) return;

            Activate();
        }

        public bool Activate()
        {
            if (IsActive) return false;

            var prefabComponent = Prefab.GetComponent<T>();
            if (prefabComponent == null) throw new Exception($"Prefab selected for pool {gameObject.name} is not poolable");

            _objectPool = new ObjectPool<T>(Size, this);
            _objectPool.Activate();
            IsActive = true;
            return true;
        }

        private void OnDisable()
        {
            if (_objectPool == null) return;

            IsActive = false;
            _objectPool.ReleaseAll();
            _objectPool = null; 
        }
        
        public T Instantiate()
        {
            var obj = Instantiate(Prefab, transform);
            obj.name = $"{Prefab.name}#{_index}";
            _index++;
            return obj.GetComponent<T>();
        }

        public T[] GetObjectsInUse()
        {
            return _objectPool.GetActiveObjects();
        }

        public bool TryGetFromPool(out T obj, Action<T> beforeActivation = null)
        {
            return _objectPool.TryGetFromPool(out obj, beforeActivation);
        }

        public bool TryGetManyFromPool(int amount, out T[] objects, Action<T, int> beforeActivation = null)
        {
            return _objectPool.GetManyFromPool(amount, out objects, beforeActivation);
        }

        public bool Release(T obj)
        {
            return _objectPool.Release(obj);
        }

        public bool ReleaseAll()
        {
            return _objectPool.ReleaseAll();
        }
    }
}
