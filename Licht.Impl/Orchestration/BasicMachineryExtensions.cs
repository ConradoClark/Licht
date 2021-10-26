using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Licht.Impl.Orchestration
{
    public static class BasicMachineryExtensions
    {
        public static BasicMachinery AddBasicMachine(this BasicMachinery basicMachinery, int priority, IEnumerable<Action> steps, Func<bool> breakCondition = null)
        {
            basicMachinery.AddMachines(new BasicMachine(priority, steps, breakCondition));
            return basicMachinery;
        }

        public static BasicMachinery AddBasicMachine(this BasicMachinery basicMachinery, int priority, IEnumerable<IEnumerable<Action>> steps, Func<bool> breakCondition = null)
        {
            basicMachinery.AddMachines(new BasicMachine(priority, steps, breakCondition));
            return basicMachinery;
        }

        public static IEnumerable<Action> AsEnumerable(this Action action)
        {
            return Enumerable.Repeat(action, 1);
        }
    }
}
