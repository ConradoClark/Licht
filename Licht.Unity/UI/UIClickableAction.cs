using Licht.Impl.Orchestration;
using Licht.Unity.Mixins;
using Licht.Unity.Objects;
using UnityEngine;

namespace Licht.Unity.UI
{
    public class UIClickableAction : BaseGameObject
    {
        [field: SerializeField]
        public UIAction Action { get; private set; }
        [field: SerializeField]
        public ClickableObjectMixin Clickable { get; private set; }

        protected override void OnAwake()
        {
            base.OnAwake();
            Clickable = new ClickableObjectMixinBuilder(this).Build();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            DefaultMachinery.AddBasicMachine(
                Clickable.HandleHover(() => Action.MenuContext.Select(Action), () => { }));

            Clickable.HandleClick(()=> Action.MenuContext.Click(Action));
        }
    }
}
