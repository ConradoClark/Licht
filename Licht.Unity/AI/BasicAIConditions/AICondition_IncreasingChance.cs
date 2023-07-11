using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Licht.Unity.PropertyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

[AddComponentMenu("L. AI Condition: Inc. Chance")]
public class AICondition_IncreasingChance : BaseAICondition
{
    [CustomLabel("Chance this condition will be met, between 0 and 100. (eg. 25 for 25%.)")]
    public float ChanceInPercentage;

    [field:SerializeField]
    public float AddWhenFalse { get; private set; }
    [field: SerializeField]
    public float AddWhenTrue { get; private set; }
    [field: SerializeField]
    public bool ResetWhenTrue { get; private set; }
    [field: SerializeField]
    public bool ResetWhenFalse { get; private set; }

    [field: SerializeField]
    [field: ReadOnly(true)]
    private float _originalChance;

    protected override void OnAwake()
    {
        base.OnAwake();
        _originalChance = ChanceInPercentage;
    }

    public override bool CheckCondition()
    {
        var result = ChanceInPercentage * 0.01f > Random.value;

        ChanceInPercentage += (result ? AddWhenTrue : AddWhenFalse);

        if ((result && ResetWhenTrue) || (!result && ResetWhenFalse))
        {
            ChanceInPercentage = _originalChance;
        }

        return result;
    }
}
