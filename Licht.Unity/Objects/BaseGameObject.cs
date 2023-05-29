using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Time;
using UnityEngine;

namespace Licht.Unity.Objects
{
    public abstract class BaseGameObject : MonoBehaviour
    {
        public BaseActor Actor { get; set; }
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

        public virtual void Init()
        {
            AttachActor();
        }

        private void AttachActor()
        {
            Actor = GetComponent<BaseActor>();
            if (Actor == null)
            {
                Actor = GetComponentInParent<BaseActor>(true);
            }
            if (Actor != null)
            {
                Actor.AddCustomObject(GetType(), this);
            }
        }

        protected virtual void OnAwake()
        {

        }

        protected virtual void OnEnable()
        {
            if (Actor == null)
            {
                AttachActor();
            }

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
