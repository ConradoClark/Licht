using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Pooling;
using UnityEngine;

namespace Licht.Unity.Pooling
{
    public abstract class EffectPoolable : MonoBehaviour, IPoolableObject
    {
        public bool ActiveOnInitialization;
        public void Initialize()
        {
            gameObject.SetActive(ActiveOnInitialization);
            if (ActiveOnInitialization) Activate();
        }

        public bool IsActive { get; set; }
        public bool Deactivate()
        {
            IsEffectOver = true;
            gameObject.SetActive(false);
            IsActive = false;
            return true;
        }

        public bool Activate()
        {
            IsEffectOver = false;

            BasicToolbox.Instance.Machinery().AddBasicMachine(HandleEffectOver());
            gameObject.SetActive(true);
            IsActive = true;

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
                    Deactivate();
                    break;
                }

                yield return TimeYields.WaitOneFrame;
            }
        }
    }
}
