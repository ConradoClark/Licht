using System;
using System.Collections.Generic;
using System.Text;
using Licht.Impl.Pooling;
using Licht.Interfaces.Pooling;
using UnityEngine;

namespace Licht.Unity.Pooling
{
    public class PrefabPool : MonoBehaviour, IPoolableObjectFactory<IPoolableObject>
    {
        public GameObject Prefab;
        public int Size;
        private ObjectPool<IPoolableObject> _objectPool;

        void OnEnable()
        {
            var prefabComponent = Prefab.GetComponent<IPoolableObject>();
            if (prefabComponent == null) throw new Exception($"Prefab selected for pool {gameObject.name} is not poolable");

            _objectPool = new ObjectPool<IPoolableObject>(Size, this);
            _objectPool.Activate();
        }

        public IPoolableObject Instantiate()
        {
            var obj = Instantiate(Prefab, transform);
            return obj.GetComponent<IPoolableObject>();
        }

        public bool TryGetFromPool(out IPoolableObject obj)
        {
            return _objectPool.TryGetFromPool(out obj);
        }

        public bool TryGetManyFromPool(int amount, out IPoolableObject[] objects)
        {
            return _objectPool.GetManyFromPool(amount, out objects);
        }
    }
}
