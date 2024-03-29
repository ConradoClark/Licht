﻿using System;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;
using Licht.Interfaces.Time;
using Licht.Unity.Builders;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using Licht.Unity.PropertyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;
using Routine = System.Collections.Generic.IEnumerable<System.Collections.Generic.IEnumerable<System.Action>>;
using Vector2 = UnityEngine.Vector2;

namespace Licht.Unity.CharacterControllers
{
    [AddComponentMenu("L. Controllers: TopDown2D Move")]
    public class LichtTopDownMoveController : LichtMovementController
    {
        [CustomHeader("Movement")]
        [BeginFoldout("Movement Properties")]
        public Vector2 LatestDirection;
        public float ChangeDirectionEventSensitivity;
        public float MaxSpeed;
        [InspectorName("Acceleration Time (s)")]
        public float AccelerationTime;
        [InspectorName("Deceleration Time (s)")]
        public float DecelerationTime;
        public EasingYields.EasingFunction MovementStartEasing;
        public EasingYields.EasingFunction MovementEndEasing;

        [field: SerializeField] public float TurnSpeedPerSecond { get; private set; } = 0.5f;
        
        [EndFoldout]
        [CustomHeader("Reference")]
        [CustomLabel("The default is the associated Actor.")]
        [CustomLabel("Select this if you want to attach this to a different Physics Object.")]
        public bool UseCustomTarget;
        [ShowWhen(nameof(UseCustomTarget))]
        public LichtPhysicsObject Target;
        public bool IsMoving { get; private set; }

        [CustomHeader("Input")]
        public InputActionReference HorizontalAxisInput;
        public InputActionReference VerticalAxisInput;
        public PlayerInput PlayerInput { get; set; }

        private LichtPhysics _physics;
        private IEventPublisher<LichtTopDownMoveEvents, LichtTopDownMoveEventArgs> _eventPublisher;

        public event Action<LichtTopDownMoveEventArgs> OnStartMoving;
        public event Action<LichtTopDownMoveEventArgs> OnStopMoving;
        public event Action<LichtTopDownMoveEventArgs> OnTopSpeed;
        public event Action<LichtTopDownMoveEventArgs> OnChangeDirection;

        public enum LichtTopDownMoveEvents
        {
            OnStartMoving,
            OnStopMoving,
            OnTopSpeed,
            OnChangeDirection,
        }

        public struct LichtTopDownMoveEventArgs
        {
            public LichtTopDownMoveController Source;
            public Vector2 Direction;
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            if (!UseCustomTarget) Target = Actor as LichtPhysicsObject;
            _physics = this.GetLichtPhysics();
            if (PlayerInput == null)
            {
                PlayerInput = SceneObject<PlayerInput>.Instance();
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _eventPublisher = this.RegisterAsEventPublisher<LichtTopDownMoveEvents, LichtTopDownMoveEventArgs>();
            _physics.LichtPhysicsMachinery.Machinery.AddBasicMachine(HandleMovement());
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            this.UnregisterAsEventPublisher<LichtTopDownMoveEvents, LichtTopDownMoveEventArgs>();
        }

        private void CheckChangeDirectionEvent(Vector2 direction)
        {
            if (direction == LatestDirection ||
                !(Vector2.Distance(direction, LatestDirection) > ChangeDirectionEventSensitivity)) return;


            var @event = new LichtTopDownMoveEventArgs
            {
                Direction = direction,
                Source = this
            };
            _eventPublisher.PublishEvent(LichtTopDownMoveEvents.OnChangeDirection, @event);
            OnChangeDirection?.Invoke(@event);
        }

        private Routine StartMovement(InputAction horizontalAction, InputAction verticalAction)
        {
            var speed = 0f;
            var lerp = new LerpBuilder(f => speed = f, () => speed)
                .SetTarget(MaxSpeed)
                .Over(AccelerationTime)
                .BreakIf(() => IsBlocked || !horizontalAction.IsPressed() && !verticalAction.IsPressed(), false)
                .Easing(MovementStartEasing)
                .UsingTimer(GameTimer)
                .Build();
            foreach (var _ in lerp)
            {
                var dir = new Vector2(horizontalAction.ReadValue<float>(), verticalAction.ReadValue<float>())
                    .normalized;

                CheckChangeDirectionEvent(dir);
                LatestDirection = dir;
                Target.ApplySpeed(LatestDirection * speed);

                yield return TimeYields.WaitOneFrameX;
            }
        }

        private Routine EndMovement(InputAction horizontalAction, InputAction verticalAction)
        {
            var speed = Math.Min(Target.LatestSpeed.magnitude, MaxSpeed);
            var lerp = new LerpBuilder(f => speed = f, () => speed)
                .SetTarget(0)
                .Over(DecelerationTime)
                .BreakIf(() => IsBlocked || horizontalAction.IsPressed() || verticalAction.IsPressed(), false)
                .Easing(MovementEndEasing)
                .UsingTimer(GameTimer)
                .Build();

            foreach (var _ in lerp)
            {
                Target.ApplySpeed(LatestDirection * speed);

                yield return TimeYields.WaitOneFrameX;
            }
        }

        private Routine Move(InputAction horizontalAction, InputAction verticalAction)
        {
            while (!IsBlocked && (horizontalAction.IsPressed() || verticalAction.IsPressed()))
            {
                var dir = new Vector2(horizontalAction.ReadValue<float>(), verticalAction.ReadValue<float>())
                    .normalized;

                CheckChangeDirectionEvent(dir);
                LatestDirection = Vector2.Lerp(LatestDirection, dir, TurnSpeedPerSecond * .166666666666667f);

                Target.ApplySpeed(LatestDirection * MaxSpeed);
                yield return TimeYields.WaitOneFrameX;
            }
        }

        private Routine HandleMovement()
        {
            var horizontalAction = PlayerInput.actions[HorizontalAxisInput.action.name];
            var verticalAction = PlayerInput.actions[VerticalAxisInput.action.name];
            while (isActiveAndEnabled)
            {
                while (IsBlocked || !horizontalAction.IsPressed() && !verticalAction.IsPressed()) yield return TimeYields.WaitOneFrameX;

                LatestDirection = new Vector2(horizontalAction.ReadValue<float>(), verticalAction.ReadValue<float>())
                    .normalized;

                var @event = new LichtTopDownMoveEventArgs
                {
                    Direction = LatestDirection,
                    Source = this
                };

                _eventPublisher.PublishEvent(LichtTopDownMoveEvents.OnStartMoving, @event);

                OnStartMoving?.Invoke(@event);

                IsMoving = true;
                yield return StartMovement(horizontalAction, verticalAction).AsCoroutine();

                if (horizontalAction.IsPressed() || verticalAction.IsPressed())
                {
                    var topSpeed = new LichtTopDownMoveEventArgs
                    {
                        Direction = LatestDirection,
                        Source = this
                    };
                    _eventPublisher.PublishEvent(LichtTopDownMoveEvents.OnTopSpeed, topSpeed);
                    OnTopSpeed?.Invoke(topSpeed);
                }

                yield return Move(horizontalAction, verticalAction).AsCoroutine();

                var stopMoving = new LichtTopDownMoveEventArgs
                {
                    Direction = LatestDirection,
                    Source = this
                };
                _eventPublisher.PublishEvent(LichtTopDownMoveEvents.OnStopMoving, stopMoving);
                OnStopMoving?.Invoke(stopMoving);

                yield return EndMovement(horizontalAction, verticalAction).AsCoroutine();
                IsMoving = false;
            }
        }
    }
}
