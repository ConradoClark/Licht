using System;
using System.Collections.Generic;
using System.Text;

namespace Licht.Unity.AI.BasicAIConditions
{
    public class AICondition_Negate : BaseAICondition
    {
        public BaseAICondition Condition;
        public override bool CheckCondition()
        {
            return !Condition.CheckCondition();
        }
    }
}
