using System;
using System.Collections.Generic;
using System.Text;

namespace Licht.Unity.AI.BasicAIConditions
{
    public class AICondition_Always : BaseAICondition
    {
        public override bool CheckCondition()
        {
            return true;
        }
    }
}
