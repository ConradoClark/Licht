using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using UnityEngine;

namespace Licht.Unity.Pooling
{
    public abstract class EffectPoolable : BaseGameObject, IPoolableComponent
    {
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

            DefaultMachinery.AddBasicMachine(HandleEffectOver());

            OnActivation();

            return true;
        }

        public abstract void OnActivation();
        public abstract bool IsEffectOver { get; protected set; }

        IEnumerable<Action> HandleEffectOver()
        {
            while (isActiveAndEnabled)
            {
                if (IsEffectOver)
                {
                    if (transform != null) transform.SetParent(_originalParent);
                    Deactivate();
                    break;
                }

                yield return TimeYields.WaitOneFrame;
            }
        }

        public MonoBehaviour Component => this;
    }
}
