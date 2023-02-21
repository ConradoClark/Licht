using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;

namespace Licht.Unity.AI.BasicAIActions
{
    public class AIAction_DoNothing : BaseAIAction
    {
        public float TimeInSeconds;
        private bool _interrupted;
        public override IEnumerable<IEnumerable<Action>> Execute(Func<bool> breakCondition)
        {
            _interrupted = false;

            yield return TimeYields.WaitSeconds(GameTimer, TimeInSeconds,
                breakCondition: () => _interrupted || breakCondition());
        }

        public override void OnInterrupt()
        {
            _interrupted = true;
        }
    }
}
