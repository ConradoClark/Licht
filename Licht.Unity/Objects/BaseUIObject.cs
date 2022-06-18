using System;
using System.Collections.Generic;
using System.Text;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Time;
using UnityEngine;

namespace Licht.Unity.Objects
{
    public class BaseUIObject : MonoBehaviour
    {
        public ITimer UITimer { get; protected set; }
        public BasicMachinery<object> DefaultMachinery { get; protected set; }

        private void Awake()
        {
            OnAwake();
            UITimer = SceneObject<DefaultUITimer>.Instance().TimerRef.Timer;
            DefaultMachinery = SceneObject<DefaultMachinery>.Instance().MachineryRef;
        }

        protected virtual void OnAwake()
        {

        }
    }
}
