using Licht.Impl.Orchestration;
using Licht.Interfaces.Time;
using Licht.Unity.Memory;
using Licht.Unity.Objects;
using UnityEngine;

namespace Licht.Unity.Mixins
{
    public abstract class MonoBehaviourMixinBuilder<TSelf, TMixin> where TSelf : MonoBehaviourMixinBuilder<TSelf, TMixin>
    {
        protected readonly MonoBehaviour SourceObject;
        protected BasicMachinery<object> DefaultMachinery;
        protected ITimer Timer;
        protected FrameVariables FrameVariables;

        public FrameVariables GetFrameVariables()
        {
            if (FrameVariables != null) return FrameVariables;
            FrameVariables = Object.FindObjectOfType<FrameVariables>();

            if (FrameVariables != null) return FrameVariables;
            var obj = new GameObject("frameVars");
            return FrameVariables = obj.AddComponent<FrameVariables>();
        }

        protected MonoBehaviourMixinBuilder(MonoBehaviour sourceObject)
        {
            SourceObject = sourceObject;
            DefaultMachinery = SceneObject<DefaultMachinery>.Instance()?.MachineryRef?.Machinery;
            Timer = SceneObject<DefaultGameTimer>.Instance().TimerRef.Timer;
            GetFrameVariables();
        }

        public TSelf WithMachinery(BasicMachinery<object> machinery)
        {
            DefaultMachinery = machinery;
            return (TSelf) this;
        }
            
        public TSelf WithTimer(ITimer timer)
        {
            Timer = timer;
            return (TSelf)this;
        }

        public abstract TMixin Build();

    }
}
