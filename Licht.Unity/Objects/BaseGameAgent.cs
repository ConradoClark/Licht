﻿using System;
using System.Collections.Generic;
using System.Text;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Time;
using UnityEngine;

namespace Licht.Unity.Objects
{
    public abstract class BaseGameAgent : BaseGameObject
    {
        [field: SerializeField]
        public ScriptTimer TimerReference { get; private set; }

        public ITimer Timer => TimerReference?.Timer ?? SceneObject<DefaultGameTimer>.Instance().TimerRef.Timer;

        protected override void OnEnable()
        {
            base.OnEnable();
            DefaultMachinery.AddUniqueMachine($"Handle_{GetInstanceID()}", UniqueMachine.UniqueMachineBehaviour.Replace,
                    Handle().AsCoroutine().RepeatUntil(() => !ComponentEnabled));
        }

        protected abstract IEnumerable<IEnumerable<Action>> Handle();

        public IEnumerable<IEnumerable<Action>> AsMachine() => Handle();
    }
}
