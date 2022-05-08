using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Orchestration;
using Licht.Unity.Builders;
using Licht.Unity.Objects;
using UnityEngine;

namespace Licht.Unity.Physics.Forces
{
    public class Gravity : LichtCustomPhysicsForce
    {
        public ScriptableLichtForceIdentifier Identifier;
        public LayerMask Affects;
        public Vector2 Direction;
        public float Speed;

        public override bool IsActive { get; set; } = true;
        public override string Key => Identifier.Name;

        protected override void OnEnable()
        {
            base.OnEnable();
            Physics.LichtPhysicsMachinery.Machinery.AddBasicMachine(StartGravity());
        }

        private IEnumerable<Action> StartGravity()
        {
            yield return TimeYields.WaitOneFrame;
            Activate();
        }

        private IEnumerable<IEnumerable<Action>> UseGravity(LichtPhysicsObject physicsObject)
        {
            while (IsActive)
            {
                while (!ActivationFlags[physicsObject] || IsBlocked(physicsObject) || Physics.GetCollisionState(physicsObject).Vertical.HitNegative) yield return TimeYields.WaitOneFrameX;

                var speed = 0f;
                foreach (var _ in new LerpBuilder(v => speed = v, () => speed)
                             .SetTarget(Speed)
                             .Over(2f)
                             .Easing(EasingYields.EasingFunction.QuadraticEaseIn)
                             .BreakIf(() => !ActivationFlags[physicsObject] || IsBlocked(physicsObject) || Physics.GetCollisionState(physicsObject).Vertical.HitNegative)
                             .UsingTimer(Physics.TimerRef.Timer)
                             .Build())
                {
                    physicsObject.ApplySpeed(Direction * speed);
                    yield return TimeYields.WaitOneFrameX;
                }

                while (IsActive && ActivationFlags[physicsObject] && !IsBlocked(physicsObject) && !Physics.GetCollisionState(physicsObject).Vertical.HitNegative)
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

