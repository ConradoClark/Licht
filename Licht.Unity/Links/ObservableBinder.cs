using Licht.Unity.Objects;
using UnityEngine;

namespace Licht.Unity.Links
{
    [AddComponentMenu("L. Link: Observable Binder")]
    public class ObservableBinder : BaseGameObject
    {
        [field: SerializeField] public ObservableBindingReference InputComponent { get; private set; }
        [field: SerializeField] public BindingReference OutputComponent { get; private set; }
        [field: SerializeField] public bool PreCacheBindings { get; private set; }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            InputComponent.OnChange.AddListener(OnInputValueChanged);
            
            if (!PreCacheBindings) return;

            InputComponent.PreCacheBindings();
            OutputComponent.PreCacheBindings();
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            InputComponent.OnChange.RemoveListener(OnInputValueChanged);
        }

        private void OnInputValueChanged(BindingReference binding)
        {
            OutputComponent.Set(binding.Get());
        }
    }
}