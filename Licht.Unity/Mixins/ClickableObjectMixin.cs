﻿using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Time;
using Licht.Unity.Memory;
using Licht.Unity.Objects;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Licht.Unity.Mixins
{
    public class ClickableObjectMixin : BaseObjectMixin
    {
        private readonly FrameVariableDefinition<bool> _clicked;
        private readonly 
            FrameVariableDefinition<bool> _hovering;
        private readonly MonoBehaviour _sourceObject;
        private readonly InputAction _clickAction;
        private readonly InputAction _mousePosInput;
        private readonly Collider2D _collider;
        private readonly Camera _camera;
        private readonly BoxCollider2D _renderTextureCollider;
        private readonly Camera _portalCamera;

        public ClickableObjectMixin(MonoBehaviour sourceObject,
            FrameVariables frameVariables,
            ITimer timer,
            BasicMachinery<object> defaultMachinery,
            Collider2D collider,
            InputActionReference clickInput,
            InputActionReference mousePosInput,
            PlayerInput playerInput,
            BoxCollider2D renderTextureCollider,
            Camera portalCamera,
            Camera camera) : base(sourceObject, frameVariables, timer, defaultMachinery)
        {
            _sourceObject = sourceObject;
            _clicked = new FrameVariableDefinition<bool>($"ClickableObjectMixin_{_sourceObject.GetInstanceID()}_Event_Clicked", CheckClick);
            _hovering = new FrameVariableDefinition<bool>($"ClickableObjectMixin_{_sourceObject.GetInstanceID()}_IsHovering", CheckHover);
            _clickAction = playerInput.actions[clickInput.action.name];
            _collider = collider;
            _camera = camera;
            _renderTextureCollider = renderTextureCollider;
            _portalCamera = portalCamera;
            _mousePosInput = playerInput.actions[mousePosInput.action.name];
        }

        public void HandleClick(Action onClick, Func<bool> breakCondition = null)
        {
            DefaultMachinery.AddBasicMachine(DoActionOnClick(() => new[] { onClick }, breakCondition));
        }

        public void HandleClick(IEnumerable<Action> onClick, Func<bool> breakCondition = null)
        {
            DefaultMachinery.AddBasicMachine(DoActionOnClick(() => onClick, breakCondition));
        }

        private IEnumerable<IEnumerable<Action>> DoActionOnClick(Func<IEnumerable<Action>> onClick, Func<bool> breakCondition)
        {
            while (_sourceObject.isActiveAndEnabled)
            {
                if (WasClickedThisFrame())
                {
                    yield return onClick();
                }
                yield return TimeYields.WaitOneFrameX;

                if (breakCondition != null && breakCondition()) break;
            }
        }

        public IEnumerable<IEnumerable<Action>> HandleHover(Action onHover, Action onExitHover)
        {
            while (_sourceObject.isActiveAndEnabled)
            {
                if (IsHovering())
                {
                    onHover();
                }
                while (IsHovering())
                {
                    yield return TimeYields.WaitOneFrameX;
                }

                onExitHover();
                while (!IsHovering())
                {
                    yield return TimeYields.WaitOneFrameX;
                }
            }
        }

        public IEnumerable<IEnumerable<Action>> HandleHover(Func<IEnumerable<Action>> onHover, Func<IEnumerable<Action>> onExitHover)
        {
            while (_sourceObject.isActiveAndEnabled)
            {
                if (IsHovering())
                {
                    yield return onHover();
                }
                while (IsHovering())
                {
                    yield return TimeYields.WaitOneFrameX;
                }

                yield return onExitHover();
                while (!IsHovering())
                {
                    yield return TimeYields.WaitOneFrameX;
                }
            }
        }

        public bool WasClickedThisFrame()
        {
            return FrameVariables.Get(_clicked);
        }

        public bool IsHovering()
        {
            return FrameVariables.Get(_hovering);
        }

        private bool CheckHover()
        {
            var pos = (Vector2)_camera.ScreenToWorldPoint(_mousePosInput.ReadValue<Vector2>());

            if (_renderTextureCollider == null) return _collider.OverlapPoint(pos);

            if (!_renderTextureCollider.OverlapPoint(pos)) return false;

            var bounds = _renderTextureCollider.bounds;
            var rectMinPosition = bounds.min;
            var rectWidth = bounds.size.x;
            var rectHeight = bounds.size.y;

            var relativePosition = pos - (Vector2)rectMinPosition;
            var normalizedPosition = new Vector2(relativePosition.x / rectWidth, relativePosition.y / rectHeight);
            normalizedPosition = new Vector2(Mathf.Clamp01(normalizedPosition.x), Mathf.Clamp01(normalizedPosition.y));

            pos = _portalCamera.ViewportToWorldPoint(normalizedPosition);
            return _collider.OverlapPoint(pos);
        }

        private bool CheckClick()
        {
            return
                _clickAction.WasPerformedThisFrame() &&
                CheckHover();
        }
    }
}
