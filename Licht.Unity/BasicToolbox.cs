﻿using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Events;
using Licht.Interfaces.Update;
using Licht.Unity.Objects;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Licht.Unity
{
    [DefaultExecutionOrder(-2000)]
    [RequireComponent(typeof(DefaultGameTimer))]
    [RequireComponent(typeof(DefaultUITimer))]
    [RequireComponent(typeof(DefaultMachinery))]
    [RequireComponent(typeof(PlayerInput))]
    public class BasicToolbox : SceneObject<BasicToolbox>
    {
        public int TargetFrameRate;
        public List<ScriptValue> ScriptableObjects;
        private IUpdateable[] _updateableScriptableObjects;
        private IInitializable[] _initializableScriptableObjects;

        protected void Awake()
        {
            if (TargetFrameRate > 0)
            {
                Application.targetFrameRate = TargetFrameRate;
                QualitySettings.vSyncCount = 0;
            }
            else
            {
                Application.targetFrameRate = -1;
            }
        }

        protected void Update()
        {
            foreach (var scriptableObject in _updateableScriptableObjects)
            {
                scriptableObject.Update();
            }
        }

        protected void OnEnable()
        {
            _updateableScriptableObjects = ScriptableObjects.Select(obj => obj.Value).OfType<IUpdateable>().ToArray();
            _initializableScriptableObjects = ScriptableObjects.Select(obj => obj.Value).OfType<IInitializable>().ToArray();

            foreach (var obj in _initializableScriptableObjects)
            {
                obj.Initialize();
            }

            DefaultMachinery.Instance().MachineryRef.Machinery.Activate();
        }

        protected void OnDisable()
        {
            DefaultMachinery.Instance().MachineryRef.Machinery.FinalizeWith(() =>
            {
                EventBroadcasterDisposer.Cleanup();
                _updateableScriptableObjects = Array.Empty<IUpdateable>();
                _initializableScriptableObjects = Array.Empty<IInitializable>();
            });
        }

        protected void OnDestroy()
        {
            EventBroadcasterDisposer.Cleanup();
            _updateableScriptableObjects = Array.Empty<IUpdateable>();
            _initializableScriptableObjects = Array.Empty<IInitializable>();
        }
    }
}
