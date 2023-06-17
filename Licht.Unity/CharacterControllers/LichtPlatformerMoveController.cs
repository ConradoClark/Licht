using System;
using System.Collections.Generic;
using Licht.Impl.Events;
using Licht.Impl.Globals;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using Licht.Unity.PropertyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Licht.Unity.CharacterControllers
{
    public class LichtPlatformerMoveController : LichtMovementController
    {
        public enum MovementTurnBehaviour
        {
            Revert,
            Skid,
        }

        public enum LichtPlatformerMoveEvents
        {
            OnStartMoving,
            OnStopMoving,
            OnTopSpeed,
            OnStartSkidding,
            OnStopSkidding,
            OnTurn
        }

        public struct LichtPlatformerMoveEventArgs
        {
            public LichtPlatformerMoveController Source;
            public float Direction;
        }

        [CustomHeader("Movement")]
        [BeginFoldout("Movement Properties")]
        public float SpeedMultiplier = 1;
        public float MaxSpeed;
        // public MovementTurnBehaviour TurnBehaviour;
        // public float MinSkidSpeed;
        [InspectorName("Acceleration Time (s)")]
        public float AccelerationTime;
        [InspectorName("Deceleration Time (s)")]
        public float DecelerationTime;
        public EasingYields.EasingFunction MovementStartEasing;
        public EasingYields.EasingFunction MovementEndEasing;

        [EndFoldout] [CustomHeader("Reference")]
        [CustomLabel("The default is the associated Actor.")]
        [CustomLabel("Select this if you want to attach this to a different Physics Object.")]
        public bool UseCustomTarget;
        [ShowWhen(nameof(UseCustomTarget))]
        public LichtPhysicsObject Target;
        public float LatestDirection { get; private set; } = 1f;

        [CustomHeader("Input")]
        public InputActionReference AxisInput;

        private LichtPhysics _physics;
        private PlayerInput _input;
        private IEventPublisher<LichtPlatformerMoveEvents, LichtPlatformerMoveEventArgs> _eventPublisher;

        protected override void OnAwake()
        {
            base.OnAwake();
            if (!UseCustomTarget) Target = Actor as LichtPhysicsObject;
            _physics = this.GetLichtPhysics();
            _input = PlayerInput.GetPlayerByIndex(0);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _physics.LichtPhysicsMachinery.Machinery.AddBasicMachine(HandleMovement());
            _eventPublisher = this.RegisterAsEventPublisher<LichtPlatformerMoveEvents, LichtPlatformerMoveEventArgs>();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            this.UnregisterAsEventPublisher<LichtPlatformerMoveEvents, LichtPlatformerMoveEventArgs>();
        }

        private IEnumerable<IEnumerable<Action>> StartMovement(InputAction moveAction, float axisSign, Func<bool> changedAxis)
        {
            yield return Target.GetSpeedAccessor()
                .X
                .SetTarget(MaxSpeed * axisSign * SpeedMultiplier)
                .Over(AccelerationTime)
                .Easing(MovementStartEasing)
                .BreakIf(() => IsBlocked || !moveAction.IsPressed() || changedAxis(), false)
                .UsingTimer(_physics.ScriptTimerRef.Timer)
                .Build();
        }

        private IEnumerable<IEnumerable<Action>> EndMovement(InputAction moveAction, Func<bool> changedAxis)
        {
            yield return Target.GetSpeedAccessor(new Vector2(Target.LatestSpeed.x, 0))
                .X
                .SetTarget(0)
                .Over(DecelerationTime)
                .Easing(MovementEndEasing)
                .BreakIf(() => IsBlocked || moveAction.IsPressed() || changedAxis(), false)
                .UsingTimer(_physics.ScriptTimerRef.Timer)
                .Build();
        }

        private IEnumerable<IEnumerable<Action>> HandleMovement()
        {
            var axisInput = _input.actions[AxisInput.action.name];
            var eventObj = new LichtPlatformerMoveEventArgs
            {
                Source = this
            };

            while (isActiveAndEnabled)
            {
                while (isActiveAndEnabled && (IsBlocked || !axisInput.IsPressed())) yield return TimeYields.WaitOneFrameX;

                if (!isActiveAndEnabled) yield break;

                var axisSign = Mathf.Sign(axisInput.ReadValue<float>());
                LatestDirection = axisSign;
                var changedAxis = new Func<bool>(() =>
                {
                    var axis = axisInput.ReadValue<float>();
                    var sign = Mathf.Sign(axis);
                    return axis != 0f && !sign.FloatEq(axisSign);
                });

                eventObj.Direction = axisSign;
                _eventPublisher.PublishEvent(LichtPlatformerMoveEvents.OnStartMoving, eventObj);

                yield return StartMovement(axisInput, axisSign, changedAxis).AsCoroutine();

                if (changedAxis())
                {
                    eventObj.Direction = -axisSign;
                    _eventPublisher.PublishEvent(LichtPlatformerMoveEvents.OnTurn, eventObj);
                }

                if (axisInput.IsPressed() && !IsBlocked && !changedAxis())
                {
                    _eventPublisher.PublishEvent(LichtPlatformerMoveEvents.OnTopSpeed, eventObj);
                }

                while (axisInput.IsPressed() && !IsBlocked && !changedAxis())
                {
                    Target.ApplySpeed(new Vector2(MaxSpeed * axisSign * SpeedMultiplier, 0));
                    yield return TimeYields.WaitOneFrameX;
                }

                if (!changedAxis()) _eventPublisher.PublishEvent(LichtPlatformerMoveEvents.OnStopMoving, eventObj);

                yield return EndMovement(axisInput, changedAxis).AsCoroutine();
            }
        }
    }
}

