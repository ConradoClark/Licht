using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Time;
using UnityEngine;

namespace Licht.Unity.Objects
{
    public class BaseGameObject : MonoBehaviour
    {
        public ITimer GameTimer { get; protected set; }
        public ITimer UITimer { get; protected set; }
        public BasicMachinery<object> DefaultMachinery { get; protected set; }

        public bool ComponentEnabled { get; private set; }

        protected bool IsDefaultHandleBehavior { get; private set; }

        private void Awake()
        {
            GameTimer = SceneObject<DefaultGameTimer>.Instance().TimerRef.Timer;
            UITimer = SceneObject<DefaultUITimer>.Instance().TimerRef.Timer;
            DefaultMachinery = SceneObject<DefaultMachinery>.Instance().MachineryRef.Machinery;
            OnAwake();
        }

        protected virtual void OnAwake()
        {

        }

        protected virtual void OnEnable()
        {
            ComponentEnabled = true;
            if (!IsDefaultHandleBehavior) DefaultMachinery.AddBasicMachine(Handle());
        }

        protected virtual void OnDisable()
        {
            ComponentEnabled = false;
        }

        protected virtual IEnumerable<IEnumerable<Action>> Handle()
        {
            IsDefaultHandleBehavior = true;
            yield break;
        }

        public virtual IEnumerable<IEnumerable<Action>> AsMachine()
        {
            yield break;
        }
    }
}
