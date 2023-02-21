using Licht.Unity.Mixins;
using Licht.Unity.Objects;
using UnityEngine.InputSystem;

public class ClickableObject : BaseGameObject
{
    public InputActionReference ClickPos;
    public InputActionReference ClickButton;
    public ClickableObjectMixin ClickableObjectMixin { get; private set; }

    protected override void OnAwake()
    {
        base.OnAwake();
        ClickableObjectMixin = new ClickableObjectMixinBuilder(this, ClickPos, ClickButton).Build();
    }
}
