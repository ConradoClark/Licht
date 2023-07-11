using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using Licht.Unity.PropertyAttributes;
using UnityEngine;

public class BaseAI : BaseGameRunner
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

    [CustomLabel("AI Patterns.")] [SerializeField]
    private AIPattern[] _patterns;

    [CustomLabel("Conditions which may break AI Patterns.")] [SerializeField]
    private BaseAICondition[] _breakConditions;

    protected override IEnumerable<IEnumerable<Action>> Handle()
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
                action.Prepare();
            }

            foreach (var action in pattern.Actions)
            {
                yield return action.Execute(() =>
                    !pattern.Enabled || !pattern.Condition.ComponentEnabled ||
                    _breakConditions.Any(cond => cond.CheckCondition())
                ).AsCoroutine();
                if (!pattern.Enabled || !pattern.Condition.ComponentEnabled) break;
            }

            break;
        }

        if (!foundPattern) yield return TimeYields.WaitOneFrameX;
    }

    public AIPattern? ChoosePattern()
    {
        foreach (var pattern in _patterns
                     .Where(pat => pat.Enabled && (pat.Condition.ComponentEnabled || pat.Condition.enabled))
                     .OrderBy(pat => pat.Priority).ToArray())
        {
            if (!pattern.Condition.CheckCondition()) continue;

            foreach (var action in pattern.Actions)
            {
                action.Prepare();
            }

            return pattern;
        }

        return null;
    }

    public IEnumerable<IEnumerable<Action>> RunPattern(AIPattern pattern)
    {

        foreach (var action in pattern.Actions)
        {
            yield return action.Execute(() =>
                !pattern.Enabled || !pattern.Condition.ComponentEnabled ||
                _breakConditions.Any(cond => cond.CheckCondition())
            ).AsCoroutine();
            if (!pattern.Enabled || !pattern.Condition.ComponentEnabled) break;
        }
    }

    public void SetPatterns(AIPattern[] patterns)
    {
        _patterns = patterns;
    }
}
