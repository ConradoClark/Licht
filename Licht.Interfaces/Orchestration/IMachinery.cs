using System.Collections.Generic;
using Licht.Interfaces.Update;

namespace Licht.Interfaces.Orchestration
{
    public interface IMachinery : IUpdateable, IActivable, IDeactivable
    {
        IReadOnlyCollection<int> ActiveMachines { get; }
        void AddMachinesWithQueue(IMachineQueue queueReference, params IMachine[] machine);
        void AddMachines(params IMachine[] machine);
        bool RemoveMachine(IMachine machine);
    }
}
