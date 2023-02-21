using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Licht.Unity.AI.BasicAIConditions
{
    public class AICondition_And : BaseAICondition
    {
        public BaseAICondition[] Conditions;
        public override bool CheckCondition()
        {
            return Conditions.All(c => c.CheckCondition());
        }
    }
}
