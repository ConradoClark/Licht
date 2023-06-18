using System.Linq;
using Licht.Unity.PropertyAttributes;

namespace Licht.Unity.AI.BasicAIConditions
{
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
