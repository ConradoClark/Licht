using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Pooling;
using Licht.Unity.Objects;
using UnityEngine;

namespace Licht.Unity.Pooling
{
    public class PooledComponent : BaseGameObject, IPoolableComponent
    {
        public event Action OnEffectOver;
        public bool ActiveOnInitialization;
        private Transform _originalParent;

        public void Initialize()
        {
            gameObject.SetActive(ActiveOnInitialization);
            _originalParent = transform.parent;
            if (ActiveOnInitialization) Activate();
        }

        public bool IsActive { get; set; }
        public bool Deactivate()
        {
            IsEffectOver = true;
            IsActive = false;

            if (this == null) return false;
            if (gameObject != null) gameObject.SetActive(false);
            return true;
        }

        public bool Activate()
        {
            IsEffectOver = false;
            gameObject.SetActive(true);
            IsActive = true;

            OnActivation();

            return true;
        }

        protected virtual void OnActivation()
        {
        }

        public virtual bool IsEffectOver { get; protected set; }

        public virtual void EndEffect()
        {
            if (this == null) return;

            IsEffectOver = true;
            OnEffectOver?.Invoke();
            if (transform != null) transform.SetParent(_originalParent);
            Deactivate();
        }

        public MonoBehaviour Component => this;
        public IPool Pool { get; set; }
    }
}
