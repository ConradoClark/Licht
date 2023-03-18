using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Pooling;
using Licht.Unity.Objects;
using UnityEngine;

namespace Licht.Unity.Pooling
{
    public class EffectPoolable : BaseGameObject, IPoolableComponent
    {
        public event Action OnEffectOver;
        public bool ActiveOnInitialization;
        private Transform _originalParent;

        public void Initialize()
        {
            _customProps = new Dictionary<string, float>();
            _customTags = new Dictionary<string, string>();
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

        public virtual void OnActivation()
        {
        }

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

        private Dictionary<string, string> _customTags;
        public Dictionary<string, string> CustomTags => _customTags;
        public IPool Pool { get; set; }

        public bool HasTag(string customTag, string value = null)
        {
            return _customTags.ContainsKey(customTag) && (value == null || _customTags[customTag] == value);
        }

        public bool HasProp(string customProp, float? value = null, float tolerance=0.01f)
        {
            return _customProps.ContainsKey(customProp) &&
                   (value == null || Mathf.Abs(_customProps[customProp] - value.Value) < tolerance);
        }
    }
}
