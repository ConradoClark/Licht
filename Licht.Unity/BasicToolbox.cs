using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Orchestration;
using Licht.Impl.Time;
using Licht.Interfaces.Update;
using UnityEngine;

namespace Licht.Unity
{
    public class BasicToolbox : MonoBehaviour
    {
        private Dictionary<Type, IUpdateable> _machineryObjects;

        public BasicMachinery<object> Machinery()
        {
            if (!_machineryObjects.ContainsKey(typeof(object)))
            {
                _machineryObjects[typeof(object)] = new BasicMachinery<object>(0);
            }

            return _machineryObjects[typeof(object)] as BasicMachinery<object>;
        }

        public BasicMachinery<TMachineryKey> Machinery<TMachineryKey>() where TMachineryKey: struct
        {
            if (!_machineryObjects.ContainsKey(typeof(TMachineryKey)))
            {
                _machineryObjects[typeof(TMachineryKey)] = new BasicMachinery<TMachineryKey>(default);
            }

            return _machineryObjects[typeof(TMachineryKey)] as BasicMachinery<TMachineryKey>;
        }

        public DefaultTimer MainTimer;

        public static BasicToolbox Instance { get; private set; }

        protected virtual void Awake()
        {
            if (Instance != null) return;
            Instance = this;
            _machineryObjects = new Dictionary<Type, IUpdateable>();
            MainTimer = new DefaultTimer(() => Time.deltaTime * 1000);
        }

        protected virtual void Update()
        {
            MainTimer.Update();
            foreach (var machinery in _machineryObjects.Values.ToArray())
            {
                machinery.Update();
            }
        }
    }
}
