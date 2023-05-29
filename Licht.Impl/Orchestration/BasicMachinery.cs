using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Licht.Impl.Events;
using Licht.Interfaces.Orchestration;

namespace Licht.Impl.Orchestration
{
    [PublicAPI]
    public class BasicMachinery<TKey> : IMachinery<TKey>
    {
        public bool IsActive { get; private set; }
        private readonly List<IMachine> _removeList = new List<IMachine>();
        private readonly Dictionary<IMachineQueue, List<IMachine>> _queuedMachines = new
            Dictionary<IMachineQueue, List<IMachine>>();

        private Dictionary<IMachineQueue, bool> _queueExhaustion = new 
            Dictionary<IMachineQueue, bool>();

        private readonly Dictionary<TKey, List<IMachine>> _machinarium;
        private readonly Dictionary<IMachine, TKey> _machineLayers;

        private TKey _defaultLayer;
        private TKey[] _layerOrder;

        private Action _finalizeAction;

        public BasicMachinery(TKey defaultValue, bool active = true)
        {
            if (defaultValue == null) throw new Exception("Machinery layer keys cannot be null.");
            IsActive = active;
            _machinarium = new Dictionary<TKey, List<IMachine>>(10000);
            _machineLayers = new Dictionary<IMachine, TKey>(100);
            _layerOrder = Array.Empty<TKey>();
            _defaultLayer = defaultValue;
        }

        public void SetLayerOrder(params TKey[] keys)
        {
            _layerOrder = keys;
        }

        public void Update()
        {
            if (_finalizeAction != null)
            {
                foreach (var machine in _machineLayers)
                {
                    machine.Key.Cleanup();
                    if (_machineLayers.ContainsKey(machine.Key) && 
                        _machinarium.ContainsKey(_machineLayers[machine.Key]))
                    {
                        _machinarium[_machineLayers[machine.Key]].Remove(machine.Key);
                    }
                }

                _finalizeAction();
                _finalizeAction = null;
            }

            if (!IsActive) return;

            _queueExhaustion = new Dictionary<IMachineQueue, bool>();
            _removeList.Clear();

            foreach (var layer in _layerOrder.Union(_machinarium.Keys))
            {
                if (!_machinarium.ContainsKey(layer)) continue;

                var count = _machinarium[layer].Count;

                for (var index = 0; index < count; index++)
                {
                    if (!_machinarium.ContainsKey(layer)) break;
                    var machine = _machinarium[layer][index];
                    var result = RunStep(machine);
                    if (result == MachineStepResult.Done) _removeList.Add(machine);
                }

                if (!_machinarium.ContainsKey(layer)) break;
            }

            foreach (var machine in _removeList.Where(machine => _machineLayers.ContainsKey(machine)))
            {
                if (!_machinarium.ContainsKey(_machineLayers[machine])) continue;
                _machinarium[_machineLayers[machine]].Remove(machine);
            }
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
                    currentQueue.Peek().Cleanup();
                    currentQueue.Dequeue();
                    break;
            }

            _queueExhaustion[currentQueue] = true;
            return result;
        }

        public void AddMachinesWithQueue(TKey layer, IMachineQueue queueReference, params IMachine[] machines)
        {            
            AddMachines(layer, machines);

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

        public void AddMachines(TKey layer, params IMachine[] machines)
        {
            var key = GetLayerKey(layer);
            foreach (var machine in machines)
            {
                if (!_machinarium.ContainsKey(key))
                {
                    _machinarium[key] = new List<IMachine>();
                }

                _machinarium[key].Add(machine);
                _machineLayers[machine] = key;
            }
        }

        public bool RemoveMachine(IMachine machine)
        {
            if (!_machineLayers.ContainsKey(machine)) return false;
            if (!_machinarium.ContainsKey(_machineLayers[machine])) return false;

            _machinarium[_machineLayers[machine]].Remove(machine);

            var currentQueue = _queuedMachines.FirstOrDefault(k => k.Key.Equals(machine.CurrentQueue)).Key;
            currentQueue?.Cancel(machine);

            return true;
        }

        public bool Deactivate()
        {
            if (!IsActive) return false;
            UniqueMachine.CleanAllUniqueMachines();
            IsActive = false;
            return true;
        }

        public bool Activate()
        {
            _finalizeAction = null;
            if (IsActive) return false;
            IsActive = true;
            return true;
        }

        private TKey GetLayerKey(TKey layer)
        {
            return layer == null || layer.Equals(default(TKey)) ? _defaultLayer : layer;
        }

        public void FinalizeWith(Action action)
        {
            IsActive = false;
            _machinarium.Clear();
            _machineLayers.Clear();
            _layerOrder = Array.Empty<TKey>();
            UniqueMachine.CleanAllUniqueMachines();

            _finalizeAction = action;
            EventBroadcasterDisposer.Cleanup();
        }
    }
}
