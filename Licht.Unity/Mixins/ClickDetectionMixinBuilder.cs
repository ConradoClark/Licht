using Licht.Unity.Memory;
using Licht.Unity.Objects;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Licht.Unity.Mixins
{
    public class ClickDetectionMixinBuilder : MonoBehaviourMixinBuilder<ClickDetectionMixinBuilder, ClickDetectionMixin>
    {
        private Camera _camera;
        private PlayerInput _playerInput;
        private readonly InputActionReference _mousePosInput;
        private readonly InputActionReference _mouseClickInput;

        public ClickDetectionMixinBuilder(MonoBehaviour sourceObject, InputActionReference mousePosInput, InputActionReference mouseClickInput) : base(sourceObject)
        {
            _mousePosInput = mousePosInput;
            _mouseClickInput = mouseClickInput;
            _playerInput = PlayerInput.GetPlayerByIndex(0);
            _camera = Camera.main;
        }

        public ClickDetectionMixinBuilder WithCamera(Camera camera)
        {
            _camera = camera;
            return this;
        }

        public ClickDetectionMixinBuilder WithPlayerInput(PlayerInput playerInput)
        {
            _playerInput = playerInput;
            return this;
        }

        public override ClickDetectionMixin Build()
        {
            return new ClickDetectionMixin(SourceObject, SceneObject<FrameVariables>.FindOrCreate("frameVars"),
                Timer, DefaultMachinery, _mouseClickInput, _mousePosInput,
                _playerInput, _camera);
        }
    }
}