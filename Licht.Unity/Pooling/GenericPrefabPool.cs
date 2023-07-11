using System;
using System.Linq;
using Licht.Impl.Pooling;
using Licht.Interfaces.Pooling;
using Licht.Interfaces.Update;
using Licht.Unity.Objects;
using UnityEngine;

namespace Licht.Unity.Pooling
{
    [DefaultExecutionOrder(-1000)]
    public class GenericPrefabPool<T> : MonoBehaviour, IPoolableObjectFactory<T>, ICanActivate
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
            var obj = InstantiateInactive(Prefab, transform);
            obj.name = $"{Prefab.name}#{_index}";
            _index++;

            var baseObjects = obj.GetComponents<BaseGameObject>()
                .Concat(obj.GetComponentsInChildren<BaseGameObject>(true)).ToArray();

            foreach (var baseObj in baseObjects)
            {
                baseObj.Init();
            }

            return obj.GetComponent<T>();
        }

        public static GameObject InstantiateInactive(GameObject original, Transform parent = null)
        {
            if (!original.activeSelf)
            {
                return Instantiate(original, parent);
            }

            var inactiveObj = new GameObject(string.Empty);
            inactiveObj.SetActive(false);

            var obj = Instantiate(original, inactiveObj.transform);
            obj.SetActive(false);
            obj.transform.SetParent(parent);
            Destroy(inactiveObj);
            return obj;
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
