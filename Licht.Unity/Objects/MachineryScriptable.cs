using Licht.Impl.Orchestration;
using UnityEngine;

namespace Licht.Unity.Objects
{
    [CreateAssetMenu(fileName = "Machinery", menuName = "Licht/Machinery/BasicMachinery", order = 1)]
    public class BasicMachineryScriptable : ScriptableValue
    {
        public override object Value => Machinery;
        public BasicMachinery<object> Machinery { get; } = new BasicMachinery<object>(0);
    }
}
