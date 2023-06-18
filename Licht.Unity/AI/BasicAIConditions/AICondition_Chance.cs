
using Licht.Unity.PropertyAttributes;
using UnityEngine;

namespace Licht.Unity.AI.BasicAIConditions
{
    public class AICondition_Chance : BaseAICondition
    {
        [CustomLabel("Chance this condition will be met, between 0 and 100. (eg. 25 for 25%.)")]
        public float ChanceInPercentage;
        public override bool CheckCondition()
        {
            return ChanceInPercentage * 0.01f > Random.value;
        }
    }
}
