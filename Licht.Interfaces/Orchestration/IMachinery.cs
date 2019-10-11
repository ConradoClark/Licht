using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
