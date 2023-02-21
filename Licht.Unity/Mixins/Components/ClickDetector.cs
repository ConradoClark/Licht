using Licht.Unity.Objects;
using UnityEngine.InputSystem;

namespace Licht.Unity.Mixins.Components
{
    public class ClickDetector : BaseGameObject
    {
        public InputActionReference ClickPos;
        public InputActionReference ClickButton;
        public ClickDetectionMixin ClickDetectionMixin { get; private set; }

        protected override void OnAwake()
        {
            base.OnAwake();
            ClickDetectionMixin = new ClickDetectionMixinBuilder(this, ClickPos, ClickButton).Build();
        }
    }
}
