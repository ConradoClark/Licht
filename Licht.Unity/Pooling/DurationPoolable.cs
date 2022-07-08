using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using UnityEngine;

namespace Licht.Unity.Pooling
{
    public class DurationPoolable: MonoBehaviour, IPoolableComponent
    {
        public ScriptBasicMachinery ScriptBasicMachineryObject;
        public ScriptTimer ScriptTimerObject;
        public bool ActiveOnInitialization;
        public int DurationInSeconds;

        public void Initialize()
        {
            gameObject.SetActive(ActiveOnInitialization);
            if (ActiveOnInitialization) Activate();
        }

        public bool IsActive => gameObject.activeSelf;
        public bool Deactivate()
        {
            gameObject.SetActive(false);
            return true;
        }

        public bool Activate()
        {
            ScriptBasicMachineryObject.Machinery.AddBasicMachine(Expire());
            gameObject.SetActive(true);
            return true;
        }

        private IEnumerable<IEnumerable<Action>> Expire()
        {
            yield return TimeYields.WaitOneFrameX;
            yield return TimeYields.WaitSeconds(ScriptTimerObject.Timer, DurationInSeconds);
            Deactivate();
        }

        public MonoBehaviour Component => this;
    }
}
