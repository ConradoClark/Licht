using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using UnityEngine;

public class BaseAI : BaseGameObject
{
    [Serializable]
    public struct AIPattern
    {
        public string Name;

        public int Priority;

        public BaseAICondition Condition;
        
        public BaseAIAction[] Actions;

        public bool Enabled;
    }

    [SerializeField]
    private AIPattern[] _patterns;

    [SerializeField]
    private BaseAICondition[] _breakConditions;

    protected override void OnEnable()
    {
        base.OnEnable();
        DefaultMachinery.AddBasicMachine(RunAI());
    }

    protected override void OnAwake()
    {
        base.OnAwake();
    }

    private IEnumerable<IEnumerable<Action>> RunAI()
    {
        while (ComponentEnabled)
        {
            var foundPattern = false;
            foreach (var pattern in _patterns
                         .Where(pat => pat.Enabled && pat.Condition.ComponentEnabled)
                         .OrderBy(pat => pat.Priority).ToArray())
            {
                if (!pattern.Condition.CheckCondition()) continue;
                foundPattern = true;

                foreach (var action in pattern.Actions)
                {
                    yield return action.Execute(() =>
                        !pattern.Enabled || !pattern.Condition.ComponentEnabled ||
                        _breakConditions.Any(cond=>cond.CheckCondition())
                    ).AsCoroutine();
                    if (!pattern.Enabled || !pattern.Condition.ComponentEnabled) break;
                }

                break;
            }

            if (!foundPattern) yield return TimeYields.WaitOneFrameX;
        }
    }
}
