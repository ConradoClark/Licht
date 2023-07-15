using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using Licht.Unity.PropertyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Licht.Unity.Links
{
    [AddComponentMenu("L. Link: Binder")]
    public class Binder : BaseGameRunner
    {
        [field: SerializeField] public BindingReference InputComponent { get; private set; }
        [field: SerializeField] public BindingReference OutputComponent { get; private set; }
        [field: SerializeField] public bool PreCacheBindings { get; private set; }

        [field: SerializeField] [field:CustomLabel("Update Frequency in ms")] public int UpdateFrequency { get; private set; } = 1;
        [field: SerializeField] public bool EventOnChange { get; private set; }

        [field:SerializeField]
        public UnityEvent<BindingReference> OnChange { get; private set; }

        private object _previousInput;

        protected override void OnEnable()
        {
            base.OnEnable();
            if (!PreCacheBindings) return;

            InputComponent.PreCacheBindings();
            OutputComponent.PreCacheBindings();
        }

        protected override IEnumerable<IEnumerable<Action>> Handle()
        {
            var inputValue = InputComponent.Get();
            if (EventOnChange && !Equals(inputValue, _previousInput))
            {
                OnChange?.Invoke(InputComponent);
            }

            _previousInput = inputValue;
            OutputComponent.Set(inputValue);

            if (UpdateFrequency <= 0)
            {
                yield return TimeYields.WaitOneFrameX;
                yield break;
            }

            yield return TimeYields.WaitMilliseconds(Timer, UpdateFrequency);
        }
    }
}