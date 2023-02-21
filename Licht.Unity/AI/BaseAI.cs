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

    private bool _enabled;

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
        _enabled = isActiveAndEnabled;
    }

    private IEnumerable<IEnumerable<Action>> RunAI()
    {
        while (_enabled)
        {
            var foundPattern = false;
            foreach (var pattern in _patterns
                         .Where(pat => pat.Enabled)
                         .OrderBy(pat => pat.Priority).ToArray())
            {
                if (!pattern.Condition.CheckCondition()) continue;
                foundPattern = true;

                foreach (var action in pattern.Actions)
                {
                    yield return action.Execute(() => _breakConditions.Any(cond=>cond.CheckCondition())
                    ).AsCoroutine();
                }

                break;
            }

            if (!foundPattern) yield return TimeYields.WaitOneFrameX;
        }
    }
}
