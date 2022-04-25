using System;
using System.Collections.Generic;
using System.Text;
using Licht.Impl.Pooling;
using Licht.Interfaces.Pooling;
using UnityEngine;

namespace Licht.Unity.Pooling
{
    public class PrefabPool : MonoBehaviour, IPoolableObjectFactory<IPoolableComponent>
    {
        public GameObject Prefab;
        public int Size;
        private ObjectPool<IPoolableComponent> _objectPool;

        void OnEnable()
        {
            var prefabComponent = Prefab.GetComponent<IPoolableComponent>();
            if (prefabComponent == null) throw new Exception($"Prefab selected for pool {gameObject.name} is not poolable");

            _objectPool = new ObjectPool<IPoolableComponent>(Size, this);
            _objectPool.Activate();
        }

        public IPoolableComponent Instantiate()
        {
            var obj = Instantiate(Prefab, transform);
            return obj.GetComponent<IPoolableComponent>();
        }

        public bool TryGetFromPool(out IPoolableComponent obj)
        {
            return _objectPool.TryGetFromPool(out obj);
        }

        public bool TryGetManyFromPool(int amount, out IPoolableComponent[] objects)
        {
            return _objectPool.GetManyFromPool(amount, out objects);
        }

        public bool Release(IPoolableComponent obj)
        {
            return _objectPool.Release(obj);
        }
    }
}
