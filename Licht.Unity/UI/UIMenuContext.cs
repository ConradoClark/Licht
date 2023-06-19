using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using Licht.Unity.PropertyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Licht.Unity.UI
{
    [AddComponentMenu("L. UI: Menu Context")]
    public class UIMenuContext : BaseGameObject
    {
        public enum UIDirection
        {
            Horizontal,
            Vertical
        }

        [CustomLabel("Direction of the menu (horizontal/vertical)")]
        public UIDirection Direction;
        [CustomLabel("Object which represents the cursor. (Optional)")]
        public UICursor Cursor;
        [CustomLabel("Select this if the cursor position of actions should be used as offset instead of their absolute value")]
        public bool UseCursorPositionAsOffset;

        [BeginFoldout("Inputs")]
        [CustomLabel("Axis input for horizontal menu movement.")]
        public InputActionReference UIHorizontal;
        [CustomLabel("Axis input for vertical menu movement.")]
        public InputActionReference UIVertical;
        [CustomLabel("Button input for confirmation.")]
        public InputActionReference UIAccept;

        [field: SerializeField]
        public bool AllowsCanceling { get; private set; }
        [ShowWhen(nameof(AllowsCanceling))]
        public InputActionReference UICancel;
        [ShowWhen(nameof(AllowsCanceling))]
        public UIAction CancelAction;

        [CustomLabel("Select this if you need to specify a particular PlayerInput")]
        public bool UseCustomPlayerInput;
        [ShowWhen(nameof(UseCustomPlayerInput))]
        public PlayerInput PlayerInput;

        private SortedList<int, UIAction> _actions;
        private KeyValuePair<int, UIAction> _currentAction;

        public event Action<UIAction> OnActionClicked;
        public event Action<UIAction> OnCursorMoved;

        protected override void OnAwake()
        {
            base.OnAwake();
            _actions ??= new SortedList<int, UIAction>();
            if (!UseCustomPlayerInput)
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
