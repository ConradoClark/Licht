using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Pooling;
using Licht.Unity.Objects;
using Licht.Unity.PropertyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Licht.Unity.Pooling
{
    [AddComponentMenu("L. Pooling: Pooled Object")]
    [RequireComponent(typeof(BaseActor))]
    public class PooledObject : BaseGameObject, IPoolableComponent
    {
        [CustomLabel("Select if this object should be activated when its pool is created.")]
        public bool ActiveOnInitialization;

        [field: CustomHeader("Events")]
        [field: SerializeField]
        public PooledComponentUnityEvents UnityEvents;
        public event Action OnRelease;
        private Transform _originalParent;

        [Serializable]
        public struct PooledComponentUnityEvents
        {
            [field:SerializeField]
            public UnityEvent OnRelease { get; private set; }
        }

        public void Initialize()
        {
            gameObject.SetActive(ActiveOnInitialization);
            _originalParent = transform.parent;
            if (ActiveOnInitialization) Activate();
        }

        public bool IsActive { get; set; }
        public bool Deactivate()
        {
            Released = true;
            IsActive = false;

            if (this == null) return false;
            if (gameObject != null) gameObject.SetActive(false);
            return true;
        }

        public bool Activate()
        {
            Released = false;
            gameObject.SetActive(true);
            IsActive = true;

            OnActivation();

            return true;
        }

        protected virtual void OnActivation()
        {
        }

        public virtual bool Released { get; protected set; }

        public virtual void Release()
        {
            if (this == null) return;

            Released = true;
            OnRelease?.Invoke();
            UnityEvents.OnRelease?.Invoke();
            if (transform != null) transform.SetParent(_originalParent);
            Deactivate();
        }
        
        public bool TryGetCustomObject<T>(out T obj, bool includeParent=true) where T : class
        {
            return Actor.TryGetCustomObject(out obj);
        }

        public BaseActor Component => Actor;
        public IPool Pool { get; set; }
    }
}
