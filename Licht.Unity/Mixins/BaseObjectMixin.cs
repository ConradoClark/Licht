using Licht.Impl.Orchestration;
using Licht.Interfaces.Time;
using Licht.Unity.Memory;
using UnityEngine;

namespace Licht.Unity.Mixins
{
    public abstract class BaseObjectMixin
    {
        protected readonly MonoBehaviour SourceObject;
        protected readonly FrameVariables FrameVariables;
        protected readonly ITimer Timer;
        protected readonly BasicMachinery<object> DefaultMachinery;

        public BaseObjectMixin(MonoBehaviour sourceObject, FrameVariables frameVars, ITimer timer,
            BasicMachinery<object> defaultMachinery)
        {
            SourceObject = sourceObject;
            FrameVariables = frameVars;
            Timer = timer;
            DefaultMachinery = defaultMachinery;
        }
    }
}
