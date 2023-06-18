using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Orchestration;
using Licht.Unity.Builders;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Licht.Unity.PropertyAttributes;
using UnityEngine;

namespace Licht.Unity.Physics.Forces
{
    public class Gravity : LichtCustomPhysicsForce
    {
        [CustomLabel("Which layers are affected by Gravity.")]
        public LayerMask Affects;

        [CustomHeader("Triggers")]
        [CustomLabel("This force's identifier in the Physics System.")]
        public ScriptIdentifier GravityIdentifier;
        [CustomLabel("Trigger for grounded. Grounded objects aren't affected by Gravity.")]
        public ScriptIdentifier GroundedIdentifier;

        [CustomHeader("Parameters")]
        public Vector2 Direction;
        public float Speed;
        [field: CustomLabel("Air time until full gravity effect.")]
        public float TimeInSecondsUntilFullEffect;

        [field:CustomLabel("Delay until gravity is activated, upon enabling it.")]
        [field:SerializeField]
        public float StartDelayInSeconds { get; private set; }

        public override bool IsActive { get; set; } = true;
        public override string Key => GravityIdentifier.name;

        protected override void OnEnable()
        {
            base.OnEnable();
            Physics.LichtPhysicsMachinery.Machinery.AddBasicMachine(StartGravity());
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Physics.OnAddPhysicsObject -= Physics_OnAddPhysicsObject;
            Physics.OnRemovePhysicsObject -= Physics_OnRemovePhysicsObject;
        }

        private IEnumerable<IEnumerable<Action>> StartGravity()
        {
            yield return TimeYields.WaitOneFrameX;
            yield return TimeYields.WaitSeconds(
                SceneObject<DefaultGameTimer>.Instance().TimerRef.Timer, StartDelayInSeconds);
            Activate();

            Physics.OnAddPhysicsObject += Physics_OnAddPhysicsObject;
            Physics.OnRemovePhysicsObject += Physics_OnRemovePhysicsObject;
        }

        private void Physics_OnRemovePhysicsObject(LichtPhysicsObject obj)
        {
            if (!Affects.Contains(obj.gameObject.layer)) return;
            ActivationFlags[obj] = false;
        }

        private void Physics_OnAddPhysicsObject(LichtPhysicsObject obj)
        {
            if (!Affects.Contains(obj.gameObject.layer) || obj.Ghost) return;
            ActivationFlags[obj] = true;
            Physics.LichtPhysicsMachinery.Machinery.AddBasicMachine(UseGravity(obj));
        }

        private bool IsGrounded(LichtPhysicsObject obj)
        {
            return obj.GetPhysicsTrigger(GroundedIdentifier);
        }

        private IEnumerable<IEnumerable<Action>> UseGravity(LichtPhysicsObject physicsObject)
        {
            while (physicsObject != null && IsActive && ActivationFlags[physicsObject])
            {
                while (ActivationFlags[physicsObject] && (IsBlocked(physicsObject) || IsGrounded(physicsObject))) yield return TimeYields.WaitOneFrameX;

                var speed = 0f;
                foreach (var _ in new LerpBuilder(v => speed = v, () => speed)
                             .SetTarget(Speed)
                             .Over(TimeInSecondsUntilFullEffect)
                             .Easing(EasingYields.EasingFunction.QuadraticEaseIn)
                             .BreakIf(() => !ActivationFlags[physicsObject] || IsBlocked(physicsObject) || IsGrounded(physicsObject))
                             .UsingTimer(Physics.ScriptTimerRef.Timer)
                             .Build())
                {
                    physicsObject.ApplySpeed(Direction * speed);
                    yield return TimeYields.WaitOneFrameX;
                }

                while (IsActive && ActivationFlags[physicsObject] && !IsBlocked(physicsObject) && !IsGrounded(physicsObject))
                {   
                    physicsObject.ApplySpeed(Direction * Speed);
                    yield return TimeYields.WaitOneFrameX;
                }
            }
        }

        public override bool Activate()
        {
            IsActive = true;

            foreach (var obj in Physics.GetDynamicObjectsByLayerMask(Affects))
            {
                if (obj.Ghost) continue;

                ActivationFlags[obj] = true;
                Physics.LichtPhysicsMachinery.Machinery.AddBasicMachine(UseGravity(obj));
            }
            return true;
        }

        public override bool Deactivate()
        {
            foreach (var flag in ActivationFlags.ToArray())
            {
                ActivationFlags[flag.Key] = false;
            }
            IsActive = false;
            return true;
        }
        public override bool ActivateFor(LichtPhysicsObject physicsObject)
        {
            ActivationFlags[physicsObject] = true;
            return true;
        }

        public override bool DeactivateFor(LichtPhysicsObject physicsObject)
        {
            ActivationFlags[physicsObject] = false;
            return true;
        }
    }
}

