using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Pooling;
using Licht.Unity.Objects;
using UnityEngine;

namespace Licht.Unity.Pooling
{
    public abstract class EffectPoolable : BaseGameObject, IPoolableComponent
    {
        public event Action OnEffectOver;
        public bool ActiveOnInitialization;
        private Transform _originalParent;

        public void Initialize()
        {
            _customProps = new Dictionary<string, float>();
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

        public abstract void OnActivation();
        public virtual bool IsEffectOver { get; protected set; }

        public virtual void EndEffect()
        {
            IsEffectOver = true;
            OnEffectOver?.Invoke();
            if (transform != null) transform.SetParent(_originalParent);
            Deactivate();
        }

        public MonoBehaviour Component => this;

        private Dictionary<string, float> _customProps;
        public Dictionary<string, float> CustomProps => _customProps;
        public IPool Pool { get; set; }
    }
}
