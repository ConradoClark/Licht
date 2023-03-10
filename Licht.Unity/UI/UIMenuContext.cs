﻿using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Licht.Unity.UI
{
    public class UIMenuContext : BaseGameObject
    {
        public enum UIDirection
        {
            Horizontal,
            Vertical
        }

        public UIDirection Direction;
        public UICursor Cursor;
        public InputActionReference UIHorizontal;
        public InputActionReference UIVertical;
        public InputActionReference UIAccept;
        public InputActionReference UICancel;
        public UIAction CancelAction;
        public PlayerInput PlayerInput;
        public bool UseCursorPositionAsOffset;

        private SortedList<int, UIAction> _actions;
        private KeyValuePair<int, UIAction> _currentAction;

        public event Action<UIAction> OnActionClicked;
        public event Action<UIAction> OnCursorMoved;

        protected override void OnAwake()
        {
            base.OnAwake();
            _actions ??= new SortedList<int, UIAction>();
            if (PlayerInput == null)
            {
                PlayerInput = SceneObject<PlayerInput>.Instance();
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            DefaultMachinery.AddBasicMachine(HandleMenu());
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _actions.Clear();
        }

        public void AddUIAction(UIAction action, int order)
        {
            _actions ??= new SortedList<int, UIAction>();
            _actions.Add(order, action);
        }

        public void RemoveUIAction(int order)
        {
            _actions ??= new SortedList<int, UIAction>();
            _actions.Remove(order);
        }

        private IEnumerable<IEnumerable<Action>> HandleMenu()
        {
            if (_actions.Count == 0) yield break;

            var uiDirection = PlayerInput.actions[Direction == UIDirection.Horizontal ? UIHorizontal.action.name : UIVertical.action.name];
            var uiAccept = PlayerInput.actions[UIAccept.action.name];
            var uiCancel = UICancel != null ? PlayerInput.actions[UICancel.action.name] : null;

            var initialSelection = _actions.FirstOrDefault(act => act.Value.Selected);

            _currentAction = initialSelection.Value != null ? initialSelection : _actions.First();
            _currentAction.Value.SetSelected();
            if (Cursor != null) Cursor.transform.localPosition = _currentAction.Value.CursorPosition;

            while (isActiveAndEnabled)
            {
                if (uiDirection.WasPerformedThisFrame())
                {
                    var dir = Mathf.Sign(uiDirection.ReadValue<float>());
                    var num = (_actions.IndexOfKey(_currentAction.Key) 
                               + ((dir > 0 && Direction == UIDirection.Vertical) ||
                                  (dir < 0 && Direction == UIDirection.Horizontal) ? -1 : +1));
                    _currentAction = _actions.Skip((num < 0 ? _actions.Count - 1 : num) % _actions.Count)
                        .FirstOrDefault();
                    if (Cursor != null) Cursor.transform.localPosition = UseCursorPositionAsOffset ?
                        (Vector2)_currentAction.Value.transform.position + _currentAction.Value.CursorPosition :
                        _currentAction.Value.CursorPosition;

                    OnCursorMoved?.Invoke(_currentAction.Value);

                    while (uiDirection.WasPerformedThisFrame())
                    {
                        yield return TimeYields.WaitOneFrameX;
                    }
                }

                if (uiAccept.WasReleasedThisFrame())
                {
                    OnActionClicked?.Invoke(_currentAction.Value);
                    yield return _currentAction.Value.DoAction().AsCoroutine();
                }

                if (uiCancel != null && uiCancel.WasReleasedThisFrame() && CancelAction != null)
                {
                    OnActionClicked?.Invoke(CancelAction);
                    yield return CancelAction.DoAction().AsCoroutine();
                }

                yield return TimeYields.WaitOneFrameX;
            }
        }

        public void Select(UIAction action)
        {
            _currentAction = _actions.FirstOrDefault(a => a.Value == action);

            if (Cursor != null) Cursor.transform.localPosition = UseCursorPositionAsOffset ?
                (Vector2)_currentAction.Value.transform.position + _currentAction.Value.CursorPosition :
                _currentAction.Value.CursorPosition;

            OnCursorMoved?.Invoke(_currentAction.Value);
        }

        public void Click(UIAction action)
        {
            _currentAction = _actions.FirstOrDefault(a => a.Value == action);
            OnActionClicked?.Invoke(_currentAction.Value);
            DefaultMachinery.AddBasicMachine(_currentAction.Value.DoAction());
        }
    }
}
