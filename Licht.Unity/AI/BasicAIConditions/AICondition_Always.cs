using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Licht.Unity.AI.BasicAIConditions
{
    [AddComponentMenu("L!> AI Condition: Always")]
    public class AICondition_Always : BaseAICondition
    {
        public override bool CheckCondition()
        {
            return true;
        }
    }
}
