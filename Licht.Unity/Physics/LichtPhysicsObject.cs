using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Memory;
using Licht.Unity.Extensions;
using Licht.Unity.Memory;
using Licht.Unity.Objects;
using Licht.Unity.Physics.CollisionDetection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Licht.Unity.Physics
{
    public class LichtPhysicsObject : BaseActor
    {
        public bool Static;
        public bool Ghost;
        public bool Sticky;

        [Serializable]
        public class TriggerDefinitions
        {
            public ScriptIdentifier TriggerName;
            public bool Triggered;
            [HideInInspector] public Object UpdatedBy;
        }

        public TriggerDefinitions[] PhysicsTriggers;
        public LichtPhysicsCollisionDetector[] CollisionDetectors;
        public List<Collider2D> AdditionalColliders;

        public LichtPhysics Physics { get; private set; }
        private string PhysicsFrameVar => $"LichtPhysicsObject_{gameObject.GetInstanceID()}";

        public bool GetPhysicsTrigger(ScriptIdentifier identifier)
        {
            return PhysicsTriggers?.FirstOrDefault(t => t.TriggerName == identifier)?.Triggered ?? false;
        }

        public bool GetPhysicsTriggerWithSource(ScriptIdentifier identifier, out Object source)
        {
            var trigger = PhysicsTriggers?.FirstOrDefault(t => t.TriggerName == identifier);
            source = trigger?.UpdatedBy;

            return trigger?.Triggered ?? false;
        }

        public void SetPhysicsTrigger(ScriptIdentifier identifier, bool value, Object updatedBy = null)
        {
            var trigger = PhysicsTriggers?.FirstOrDefault(t => t.TriggerName == identifier);
            if (trigger == null) throw new Exception($"Trigger {identifier.name} not defined in Physics Object.");

            trigger.Triggered = value;
            trigger.UpdatedBy = updatedBy;
        }

        public Vector2 Speed
        {
            get
            {
                return Physics.GetFrameVariables()
                    .Get(new FrameVariableDefinition<Vector2>(PhysicsFrameVar, () => Vector2.zero));
            }
            set
            {
                Physics.GetFrameVariables().Set(PhysicsFrameVar, value);
                _latestSpeed.Current = value;
                if (value != Vector2.zero)
                {
                    _latestNonZeroSpeed.Current = value;
                }
            }
        }

        public Vector2 CalculatedSpeed { get; set; }

        private Caterpillar<Vector2> _latestNonZeroSpeed;
        public Vector2 LatestNonZeroSpeed => _latestNonZeroSpeed.Current;

        private Caterpillar<Vector2> _latestSpeed;
        public Vector2 LatestSpeed => _latestSpeed.Current;

        private Caterpillar<Vector2> _latestDirection;
        public Vector2 LatestDirection => _latestDirection.Current;

        protected override void OnAwake()
        {
            Physics = this.GetLichtPhysics();
            _latestSpeed = new Caterpillar<Vector2>
            {
                TailSize = 1
            };

            _latestNonZeroSpeed = new Caterpillar<Vector2>
            {
                TailSize = 1
            };

            _latestDirection = new Caterpillar<Vector2>
            {
                TailSize = 1
            };

            foreach (var additionalCollider in AdditionalColliders)
            {
                Physics.RegisterCollider(additionalCollider, this);
            }
        }

        public void ImplyDirection(Vector2 direction)
        {
            _latestDirection.Current = new Vector2(Mathf.Abs(direction.x) > 0 ? direction.x : _latestDirection.Current.x,
                Mathf.Abs(direction.y) > 0 ? direction.y : _latestDirection.Current.y);
        }

        public void ApplySpeed(Vector2 speed)
        {
            Speed += speed;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Physics.AddPhysicsObject(this);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Physics.RemovePhysicsObject(this);
            foreach (var trigger in PhysicsTriggers)
            {
                SetPhysicsTrigger(trigger.TriggerName, false);
            }
        }

        public void CheckCollision(LichtPhysicsCollisionDetector.CollisionDetectorType type)
        {
            foreach (var detector in CollisionDetectors.Where(cd => cd.DetectorType 
                                                                    == type))
            {
                if (!detector.ComponentEnabled) continue;
                detector.AttachPhysicsObject(this);
                if (detector.IsBlocked || !detector.isActiveAndEnabled) continue;

                detector.CheckCollision();

                var result = detector.Clamp();
                if (type == LichtPhysicsCollisionDetector.CollisionDetectorType.PreUpdate) CalculatedSpeed = result;
                else transform.position = result;
            }
        }
    }
}
