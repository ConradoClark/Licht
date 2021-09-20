using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Licht.Interfaces.Orchestration;

namespace Licht.Impl.Orchestration
{
    [PublicAPI]
    public class BasicMachinery : IMachinery
    {
        public bool IsActive { get; private set; }
        public IReadOnlyCollection<int> ActiveMachines => _activeMachines;
        private readonly List<int> _activeMachines = new List<int>();
        private readonly List<int> _removeList = new List<int>();
        private readonly IMachine[] _machinarium;
        private readonly Dictionary<IMachineQueue, List<IMachine>> _queuedMachines = new
            Dictionary<IMachineQueue, List<IMachine>>();

        private Dictionary<IMachineQueue, bool> _queueExhaustion = new 
            Dictionary<IMachineQueue, bool>();

        public BasicMachinery(bool active = true, int machineSize = 5000)
        {
            IsActive = active;
            _machinarium = new IMachine[machineSize];
        }

        public void Update()
        {
            if (!IsActive) return;

            _queueExhaustion = new Dictionary<IMachineQueue, bool>();
            _activeMachines.Sort((i1, i2) => i2.CompareTo(i1));
            _removeList.Clear();

            var stack = new Stack<int>(_activeMachines);
            while (stack.Count > 0)
            {
                var makineryIndex = stack.Pop();
                IMachine m = _machinarium[makineryIndex];
                var result = RunStep(m);

                if (result != MachineStepResult.Done) continue;

                _machinarium[makineryIndex] = null;
                _removeList.Add(makineryIndex);
            }

            _activeMachines.RemoveAll(r => _removeList.Contains(r));
        }

        private MachineStepResult RunStep(IMachine m)
        {
            var currentQueue = _queuedMachines.FirstOrDefault(k => k.Key.Equals(m.CurrentQueue)).Key;
            if (currentQueue == null) return m.RunStep();
            if (_queueExhaustion.ContainsKey(currentQueue) && _queueExhaustion[currentQueue]) return MachineStepResult.QueueWaiting;
            if (currentQueue.IsEmpty) return MachineStepResult.Done;
            if (m != currentQueue.Peek()) return MachineStepResult.QueueWaiting;

            var result = m.RunStep();
            switch (result)
            {
                case MachineStepResult.Skip:
                    {
                        currentQueue.Dequeue();
                        if (!currentQueue.IsEmpty)
                        {
                            return RunStep(currentQueue.Peek());
                        }
                        break;
                    }
                case MachineStepResult.Done:
                    currentQueue.Dequeue();
                    break;
            }

            _queueExhaustion[currentQueue] = true;
            return result;
        }

        public void AddMachinesWithQueue(IMachineQueue queueReference, params IMachine[] machines)
        {            
            AddMachines(machines);

            if (!_queuedMachines.ContainsKey(queueReference))
            {
                _queuedMachines[queueReference] = machines.ToList();
            }
            else
            {
                _queuedMachines[queueReference].AddRange(machines);       
            }

            foreach (var machine in machines)
            {
                machine.CurrentQueue = queueReference;
                queueReference.Enqueue(machine);
            }
        }

        public void AddMachines(params IMachine[] machines)
        {
            foreach (var machine in machines)
            {
                if (machine.Priority < 1) continue;
                machine.AssignedPriority = (_machinarium.Skip(machine.Priority - 1)
                                               .Select((m, ix) => new { m, ix })
                                               .FirstOrDefault(r => r.m == null)?.ix).GetValueOrDefault() 
                                           + machine.Priority - 1;

                if (_machinarium[machine.AssignedPriority] != null) continue;

                _machinarium[machine.AssignedPriority] = machine;
                _activeMachines.Add(machine.AssignedPriority);
            }
        }

        public bool RemoveMachine(IMachine machine)
        {
            if (_machinarium[machine.AssignedPriority] != machine) return false;

            _machinarium[machine.AssignedPriority] = null;
            _activeMachines.Remove(machine.AssignedPriority);

            var currentQueue = _queuedMachines.FirstOrDefault(k => k.Key.Equals(machine.CurrentQueue)).Key;
            currentQueue?.Cancel(machine);

            return true;

        }

        public bool Deactivate()
        {
            if (!IsActive) return false;
            IsActive = false;
            return true;
        }

        public bool Activate()
        {
            if (IsActive) return false;
            IsActive = true;
            return true;
        }
    }
}
