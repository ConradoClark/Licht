using System;
using System.Collections.Generic;
using System.Text;
using Licht.Unity.PropertyAttributes;
using UnityEngine;

namespace Licht.Unity.AI.BasicAIConditions
{
    [AddComponentMenu("L!> AI Condition: Negate")]
    public class AICondition_Negate : BaseAICondition
    {
        [CustomLabel("Returns the opposite of the result of this condition.")]
        public BaseAICondition Condition;
        public override bool CheckCondition()
        {
            return !Condition.CheckCondition();
        }
    }
}
