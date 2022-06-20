﻿using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Licht.Unity.UI
{
    public class UIMenuContext : BaseUIObject
    {
        public enum UIDirection
        {
            Horizontal,
            Vertical
        }

        public UIDirection Direction;
        public UICursor Cursor;
        public ScriptInput UIHorizontal;
        public ScriptInput UIVertical;
        public ScriptInput UIAccept;
        public ScriptInput UICancel;
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
        }

        private void OnEnable()
        {
            DefaultMachinery.AddBasicMachine(HandleMenu());
        }

        private void OnDisable()
        {
            _actions.Clear();
        }

        public void AddUIAction(UIAction action, int order)
        {
            _actions ??= new SortedList<int, UIAction>();
            _actions.Add(order, action);
        }

        private IEnumerable<IEnumerable<Action>> HandleMenu()
        {
            if (_actions.Count == 0) yield break;

            var uiDirection = PlayerInput.actions[Direction == UIDirection.Horizontal ? UIHorizontal.ActionName : UIVertical.ActionName];
            var uiAccept = PlayerInput.actions[UIAccept.ActionName];
            var uiCancel = PlayerInput.actions[UICancel.ActionName];
            _currentAction = _actions.First();
            _currentAction.Value.SetSelected();
            Cursor.transform.localPosition = _currentAction.Value.CursorPosition;

            while (isActiveAndEnabled)
            {
                if (uiDirection.WasPerformedThisFrame())
                {
                    var dir = Mathf.Sign(uiDirection.ReadValue<float>());
                    var num = (_actions.IndexOfKey(_currentAction.Key) + (dir > 0 ? -1 : +1));
                    _currentAction = _actions.Skip((num < 0 ? _actions.Count - 1 : num) % _actions.Count)
                        .FirstOrDefault();
                    Cursor.transform.localPosition = UseCursorPositionAsOffset ?
                        (Vector2)_currentAction.Value.transform.position + _currentAction.Value.CursorPosition :
                        _currentAction.Value.CursorPosition;

                    OnCursorMoved?.Invoke(_currentAction.Value);
                }

                if (uiAccept.WasPerformedThisFrame())
                {
                    OnActionClicked?.Invoke(_currentAction.Value);
                    yield return _currentAction.Value.DoAction().AsCoroutine();
                }

                if (uiCancel.WasPerformedThisFrame() && CancelAction != null)
                {
                    OnActionClicked?.Invoke(CancelAction);
                    yield return CancelAction.DoAction().AsCoroutine();
                }

                yield return TimeYields.WaitOneFrameX;
            }
        }
    }
}