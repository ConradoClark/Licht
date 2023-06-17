using System;
using System.Collections.Generic;
using System.Text;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Time;
using Licht.Unity.PropertyAttributes;
using UnityEngine;

namespace Licht.Unity.Objects
{
    public abstract class BaseGameRunner : BaseGameObject
    {
        [field: SerializeField]
        public bool RunOnEnable { get; private set; }

        [field: SerializeField]
        public bool Loop { get; private set; }

        [field:SerializeField]
        public bool UseCustomTimer { get; private set; }

        [field: ShowWhen(nameof(UseCustomTimer))]
        [field: SerializeField]
        public ScriptTimer TimerReference { get; private set; }

        public ITimer Timer => TimerReference?.Timer ?? SceneObject<DefaultGameTimer>.Instance().TimerRef.Timer;

        protected override void OnEnable()
        {
            base.OnEnable();
            if (RunOnEnable)
            {
                DefaultMachinery.AddUniqueMachine($"Handle_{GetInstanceID()}", UniqueMachine.UniqueMachineBehaviour.Replace,
                    Loop ? Handle().AsCoroutine().RepeatUntil(() => !ComponentEnabled) : Handle().AsCoroutine());
            }
        }

        protected abstract IEnumerable<IEnumerable<Action>> Handle();

        public IEnumerable<IEnumerable<Action>> AsMachine() => Handle();
    }
}
