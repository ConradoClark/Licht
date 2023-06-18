using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using Licht.Unity.PropertyAttributes;
using UnityEngine;

public class BaseAI : BaseGameObject
{
    [Serializable]
    public struct AIPattern
    {
        public string Name;

        [CustomLabel("Lower number has higher priority.")]
        public int Priority;

        [CustomLabel("Actions will trigger when condition is met.")]
        public BaseAICondition Condition;
        
        [CustomLabel("Actions occur in the order they are configured.")]
        public BaseAIAction[] Actions;

        public bool Enabled;
    }

    [CustomLabel("AI Patterns.")]
    [SerializeField]
    private AIPattern[] _patterns;

    [CustomLabel("Conditions which may break AI Patterns.")]
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
