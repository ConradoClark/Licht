using Licht.Impl.Orchestration;
using Licht.Interfaces.Time;
using Licht.Unity.Memory;
using Licht.Unity.Objects;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Licht.Unity.Mixins
{
    public class ClickDetectionMixin: BaseObjectMixin
    {
        private readonly InputAction _clickAction;
        private readonly InputAction _mousePosInput;
        private readonly Camera _camera;

        public ClickDetectionMixin(MonoBehaviour sourceObject,
            FrameVariables frameVariables,
            ITimer timer,
            BasicMachinery<object> defaultMachinery,
            ScriptInput clickInput,
            ScriptInput mousePosInput,
            PlayerInput playerInput,
            Camera camera) : base(sourceObject, frameVariables, timer, defaultMachinery)
        {
            _camera = camera;
            _clickAction = playerInput.actions[clickInput.ActionName];
            _mousePosInput = playerInput.actions[mousePosInput.ActionName];
        }

        public bool WasClickedThisFrame(out Vector3 clickPositionWorld)
        {
            if (_clickAction.WasPerformedThisFrame())
            {
                var pos = _camera.ScreenToWorldPoint(_mousePosInput.ReadValue<Vector2>());
                clickPositionWorld = pos;

                return true;
            }

            clickPositionWorld = Vector3.zero;
            return false;
        }

        public bool IsPressed(out Vector3 clickPositionWorld)
        {
            if (_clickAction.IsPressed())
            {
                var pos = _camera.ScreenToWorldPoint(_mousePosInput.ReadValue<Vector2>());
                clickPositionWorld = pos;

                return true;
            }

            clickPositionWorld = Vector3.zero;
            return false;
        }

        public Vector3 GetMousePosition()
        {
            return _camera.ScreenToWorldPoint(_mousePosInput.ReadValue<Vector2>());
        }
    }
}
