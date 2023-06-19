using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Licht.Unity.PropertyAttributes;
using UnityEngine;

namespace Licht.Unity.AI.BasicAIConditions
{
    [AddComponentMenu("L!> AI Condition: And")]
    public class AICondition_And : BaseAICondition
    {
        [CustomLabel("Logical AND amongst selected conditions.")]
        public BaseAICondition[] Conditions;
        public override bool CheckCondition()
        {
            return Conditions.All(c => c.CheckCondition());
        }
    }
}
