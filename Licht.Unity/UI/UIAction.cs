using System;
using System.Collections.Generic;
using Licht.Unity.Objects;
using UnityEngine;

namespace Licht.Unity.UI
{
    [DefaultExecutionOrder(-1)]
    public abstract class UIAction : BaseGameObject
    {
        public bool Selected;
        public int Order;
        public UIMenuContext MenuContext;
        public Vector2 CursorPosition;
        public abstract IEnumerable<IEnumerable<Action>> DoAction();
        public abstract void OnSelect(bool manual);
        public abstract void OnDeselect();
        public abstract void OnInit();
        public virtual bool IsBlocked { get; }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (IsBlocked) return;
            MenuContext.AddUIAction(this, Order);
            MenuContext.OnCursorMoved += MenuContext_OnCursorMoved;
            OnInit();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            MenuContext.OnCursorMoved -= MenuContext_OnCursorMoved;
            MenuContext.RemoveUIAction(Order);
        }

        public void SetSelected()
        {
            OnSelect(false);
        }

        private void MenuContext_OnCursorMoved(UIAction obj)
        {
            if (obj.GetInstanceID() == GetInstanceID()) OnSelect(true);
            else if (Selected) OnDeselect();
        }
    }
}
