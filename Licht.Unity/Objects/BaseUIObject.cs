using System;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Time;
using UnityEngine;

namespace Licht.Unity.Objects
{
    [Obsolete]
    public class BaseUIObject : MonoBehaviour
    {
        public ITimer UITimer { get; protected set; }
        public BasicMachinery<object> DefaultMachinery { get; protected set; }

        private void Awake()
        {
            UITimer = SceneObject<DefaultUITimer>.Instance().TimerRef.Timer;
            DefaultMachinery = SceneObject<DefaultMachinery>.Instance().MachineryRef.Machinery;
            OnAwake();
        }

        protected virtual void OnAwake()
        {

        }
    }
}
