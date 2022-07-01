using Licht.Unity.Memory;
using Licht.Unity.Objects;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Licht.Unity.Mixins
{
    public class ClickableObjectMixinBuilder : MonoBehaviourMixinBuilder<ClickableObjectMixinBuilder, ClickableObjectMixin>
    {
        private Camera _camera;
        private Collider2D _collider;
        private PlayerInput _playerInput;
        private readonly ScriptInput _mousePosInput;
        private readonly ScriptInput _mouseClickInput;
        
        public ClickableObjectMixinBuilder(MonoBehaviour sourceObject, ScriptInput mousePosInput, ScriptInput mouseClickInput) : base(sourceObject)
        {
            _mousePosInput = mousePosInput;
            _mouseClickInput = mouseClickInput;
            _playerInput = PlayerInput.GetPlayerByIndex(0);
            _collider = SourceObject?.GetComponent<Collider2D>();
            _camera = Camera.main;
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
