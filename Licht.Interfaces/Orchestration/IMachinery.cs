using System.Collections.Generic;
using Licht.Interfaces.Update;

namespace Licht.Interfaces.Orchestration
{
    public interface IMachinery<in TKey> : IUpdateable, IActivable, IDeactivable
    {
        void AddMachinesWithQueue(TKey layer, IMachineQueue queueReference, params IMachine[] machine);
        void AddMachines(TKey layer, params IMachine[] machine);
        bool RemoveMachine(IMachine machine);
    }
}
