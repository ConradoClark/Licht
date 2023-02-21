using System.Linq;

namespace Licht.Unity.AI.BasicAIConditions
{
    public class AICondition_Or : BaseAICondition
    {
        public BaseAICondition[] Conditions;
        public override bool CheckCondition()
        {
            return Conditions.Any(c => c.CheckCondition());
        }
    }
}
