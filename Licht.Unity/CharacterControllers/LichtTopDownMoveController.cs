using System;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;
using Licht.Unity.Builders;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using UnityEngine.InputSystem;
using Routine = System.Collections.Generic.IEnumerable<System.Collections.Generic.IEnumerable<System.Action>>;
using Vector2 = UnityEngine.Vector2;

namespace Licht.Unity.CharacterControllers
{
    public class LichtTopDownMoveController : LichtMovementController
    {
        public float ChangeDirectionEventSensitivity;
        public float MaxSpeed;
        public float AccelerationTime;
        public float DecelerationTime;
        public EasingYields.EasingFunction MovementStartEasing;
        public EasingYields.EasingFunction MovementEndEasing;
        public LichtPhysicsObject Target;
        public Vector2 LatestDirection;
        public bool IsMoving { get; private set; }

        public InputActionReference HorizontalAxisInput;
        public InputActionReference VerticalAxisInput;
        public PlayerInput PlayerInput;

        public ScriptTimer TimerRef;

        private LichtPhysics _physics;
        private IEventPublisher<LichtTopDownMoveEvents, LichtTopDownMoveEventArgs> _eventPublisher;

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
            _physics = this.GetLichtPhysics();
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
            if (direction != LatestDirection && Vector2.Distance(direction, LatestDirection) > ChangeDirectionEventSensitivity)
            {
                _eventPublisher.PublishEvent(LichtTopDownMoveEvents.OnChangeDirection, new LichtTopDownMoveEventArgs
                {
                    Direction = direction,
                    Source = this
                });
            }
        }

        private Routine StartMovement(InputAction horizontalAction, InputAction verticalAction)
        {
            var speed = 0f;
            var lerp = new LerpBuilder(f => speed = f, () => speed)
                .SetTarget(MaxSpeed)
                .Over(AccelerationTime)
                .BreakIf(() => IsBlocked || !horizontalAction.IsPressed() && !verticalAction.IsPressed(), false)
                .Easing(MovementStartEasing)
                .UsingTimer(TimerRef.Timer)
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
                .UsingTimer(TimerRef.Timer)
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
                LatestDirection = dir;

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

                _eventPublisher.PublishEvent(LichtTopDownMoveEvents.OnStartMoving, new LichtTopDownMoveEventArgs
                {
                    Direction = LatestDirection,
                    Source = this
                });

                IsMoving = true;
                yield return StartMovement(horizontalAction, verticalAction).AsCoroutine();

                if (horizontalAction.IsPressed() || verticalAction.IsPressed())
                {
                    _eventPublisher.PublishEvent(LichtTopDownMoveEvents.OnTopSpeed, new LichtTopDownMoveEventArgs
                    {
                        Direction = LatestDirection,
                        Source = this
                    });
                }

                yield return Move(horizontalAction, verticalAction).AsCoroutine();

                _eventPublisher.PublishEvent(LichtTopDownMoveEvents.OnStopMoving, new LichtTopDownMoveEventArgs
                {
                    Direction = LatestDirection,
                    Source = this
                });

                yield return EndMovement(horizontalAction, verticalAction).AsCoroutine();
                IsMoving = false;
            }
        }
    }
}
