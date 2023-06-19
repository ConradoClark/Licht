using System.Linq;
using Licht.Unity.PropertyAttributes;
using UnityEngine;

namespace Licht.Unity.AI.BasicAIConditions
{
    [AddComponentMenu("L. AI Condition: Or")]
    public class AICondition_Or : BaseAICondition
    {
        [CustomLabel("Logical OR amongst selected conditions.")]
        public BaseAICondition[] Conditions;
        public override bool CheckCondition()
        {
            return Conditions.Any(c => c.CheckCondition());
        }
    }
}
