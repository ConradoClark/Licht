using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Licht.Impl.Orchestration
{
    public static class BasicMachineryExtensions
    {
        public static BasicMachinery AddBasicMachine(this BasicMachinery basicMachinery, int priority, IEnumerable<Action> steps)
        {
            basicMachinery.AddMachines(new BasicMachine(priority, steps));
            return basicMachinery;
        }
    }
}
