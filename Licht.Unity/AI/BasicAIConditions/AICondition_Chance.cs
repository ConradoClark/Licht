
using UnityEngine;

namespace Licht.Unity.AI.BasicAIConditions
{
    public class AICondition_Chance : BaseAICondition
    {
        public int ChanceInPercentage;
        public override bool CheckCondition()
        {
            return ChanceInPercentage * 0.01f > Random.value;
        }
    }
}
