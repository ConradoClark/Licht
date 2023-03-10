using Licht.Unity.Memory;
using Licht.Unity.Objects;
using Licht.Unity.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Licht.Unity.Mixins
{
    public class ClickableObjectMixinBuilder : MonoBehaviourMixinBuilder<ClickableObjectMixinBuilder, ClickableObjectMixin>
    {
        private Camera _camera;
        private Collider2D _collider;
        private PlayerInput _playerInput;
        private readonly InputActionReference _mousePosInput;
        private readonly InputActionReference _mouseClickInput;
        
        public ClickableObjectMixinBuilder(MonoBehaviour sourceObject, InputActionReference mousePosInput = null, 
            InputActionReference mouseClickInput = null, bool rightClick = false) : base(sourceObject)
        {
            _mousePosInput = mousePosInput ?? SceneObject<DefaultMouseInputs>.Instance()?.MousePos;
            _mouseClickInput = mouseClickInput ?? rightClick ? SceneObject<DefaultMouseInputs>.Instance()?.MouseRightClick 
                : SceneObject<DefaultMouseInputs>.Instance()?.MouseLeftClick;
            _playerInput = PlayerInput.GetPlayerByIndex(0);
            _collider = SourceObject?.GetComponent<Collider2D>();
            _camera = SceneObject<UICamera>.Instance()?.Camera ?? Camera.main;
        }

        public ClickableObjectMixinBuilder WithCamera(Camera camera)
        {
            _camera = camera;
            return this;
        }

        public ClickableObjectMixinBuilder WithCollider(Collider2D collider)
        {
            _collider = collider;
            return this;
        }

        public ClickableObjectMixinBuilder WithPlayerInput(PlayerInput playerInput)
        {
            _playerInput = playerInput;
            return this;
        }

        public override ClickableObjectMixin Build()
        {
            return new ClickableObjectMixin(SourceObject, SceneObject<FrameVariables>.FindOrCreate("frameVars"), 
                Timer, DefaultMachinery, _collider, _mouseClickInput, _mousePosInput,
                _playerInput, _camera);
        }
    }
}
