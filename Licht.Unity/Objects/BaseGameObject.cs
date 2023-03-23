using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Time;
using UnityEngine;

namespace Licht.Unity.Objects
{
    public abstract class BaseGameObject : MonoBehaviour
    {
        public ITimer GameTimer { get; protected set; }
        public ITimer UITimer { get; protected set; }
        public BasicMachinery<object> DefaultMachinery { get; protected set; }

        public bool ComponentEnabled { get; private set; }

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
            GameTimer ??= SceneObject<DefaultGameTimer>.Instance().TimerRef.Timer;
            UITimer ??= SceneObject<DefaultUITimer>.Instance().TimerRef.Timer;
            DefaultMachinery ??= SceneObject<DefaultMachinery>.Instance().MachineryRef.Machinery;
            ComponentEnabled = true;
        }

        protected virtual void OnDisable()
        {
            ComponentEnabled = false;
        }
    }
}
