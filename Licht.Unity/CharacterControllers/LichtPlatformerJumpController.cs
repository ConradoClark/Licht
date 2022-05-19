using System;
using System.Collections.Generic;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Licht.Unity.CharacterControllers
{
    public class LichtPlatformerJumpController : LichtMovementController
    {
        public enum LichtPlatformerJumpEvents
        {
            OnJumpStart,
            OnJumpEnd,
        }

        public struct LichtPlatformerJumpEventArgs
        {
            public LichtPlatformerJumpController Source;
        }

        public ScriptableLichtForceIdentifier GravityIdentifier;
        public ScriptableInputAction JumpInput;
        public float JumpSpeed;
        public float AccelerationTime;
        public float DecelerationTime;
        public EasingYields.EasingFunction MovementStartEasing;
        public EasingYields.EasingFunction MovementEndEasing;
        public LichtPhysicsObject Target;
        public float InputBufferTime;
        public float CoyoteJumpTime;

        public bool IsJumping { get; protected set; }

        private LichtPhysics _physics;
        private PlayerInput _input;
        private IEventPublisher<LichtPlatformerJumpEvents, LichtPlatformerJumpEventArgs> _eventPublisher;

        protected override void Awake()
        {
            base.Awake();
            _physics = this.GetLichtPhysics();
            _input = PlayerInput.GetPlayerByIndex(0);
        }

        private void OnEnable()
        {
            _physics.LichtPhysicsMachinery.Machinery.AddBasicMachine(HandleJump());
            _eventPublisher = this.RegisterAsEventPublisher<LichtPlatformerJumpEvents, LichtPlatformerJumpEventArgs>();
        }

        private void OnDisable()
        {
            this.UnregisterAsEventPublisher<LichtPlatformerJumpEvents, LichtPlatformerJumpEventArgs>();
        }

        private IEnumerable<IEnumerable<Action>> ExecuteJump()
        {
            IsJumping = true;
            _eventPublisher.PublishEvent(LichtPlatformerJumpEvents.OnJumpStart, new LichtPlatformerJumpEventArgs
            {
                Source = this
            });
            _physics.BlockCustomPhysicsForceForObject(this, Target, GravityIdentifier.Name);
            yield return TimeYields.WaitOneFrameX;

            yield return Target.GetSpeedAccessor()
                .Y
                .SetTarget(JumpSpeed)
                .Over(AccelerationTime)
                .Easing(MovementStartEasing)
                .BreakIf(() => _physics.GetCollisionState(Target).Vertical.HitPositive || IsBlocked, false)
                .UsingTimer(_physics.TimerRef.Timer)
                .Build();

            yield return Target.GetSpeedAccessor(new Vector2(0, Target.LatestSpeed.y))
                .Y
                .SetTarget(0)
                .Over(DecelerationTime)
                .Easing(MovementEndEasing)
                .BreakIf(() => _physics.GetCollisionState(Target).Vertical.HitPositive || IsBlocked, false)
                .UsingTimer(_physics.TimerRef.Timer)
                .Build();

            yield return TimeYields.WaitOneFrameX;

            IsJumping = false;
            _physics.UnblockCustomPhysicsForceForObject(this, Target, GravityIdentifier.Name);
            _eventPublisher.PublishEvent(LichtPlatformerJumpEvents.OnJumpEnd, new LichtPlatformerJumpEventArgs
            {
                Source = this
            });
        }

        protected virtual IEnumerable<IEnumerable<Action>> HandleJump()
        {
            var jumpInput = _input.actions[JumpInput.ActionName];
            while (isActiveAndEnabled)
            {
                var collisionState = _physics.GetCollisionState(Target);
                var jumped = false;
                while (!collisionState.Vertical.HitNegative)
                {
                    if (IsBlocked)
                    {
                        yield return TimeYields.WaitOneFrameX;
                        continue;
                    }

                    if (InputBufferTime > 0 && jumpInput.WasPerformedThisFrame())
                    {
                        foreach (var _ in TimeYields.WaitSeconds(_physics.TimerRef.Timer, InputBufferTime))
                        {
                            collisionState = _physics.GetCollisionState(Target);
                            if (!collisionState.Vertical.HitNegative || IsBlocked)
                            {
                                yield return TimeYields.WaitOneFrameX;
                                continue;
                            }

                            jumped = true;
                            yield return ExecuteJump().AsCoroutine();
                            break;
                        }
                    }
                    else
                    {
                        yield return TimeYields.WaitOneFrameX;
                    }

                    if (jumped) continue;

                    collisionState = _physics.GetCollisionState(Target);
                }

                if (CoyoteJumpTime > 0 && !jumped)
                {
                    foreach (var _ in TimeYields.WaitSeconds(_physics.TimerRef.Timer, CoyoteJumpTime))
                    {
                        if (!jumpInput.WasPerformedThisFrame() || IsBlocked)
                        {
                            yield return TimeYields.WaitOneFrameX;
                            continue;
                        }

                        jumped = true;
                        yield return ExecuteJump().AsCoroutine();
                        break;
                    }
                }

                if (jumpInput.WasPerformedThisFrame() && !IsBlocked && !jumped)
                {
                    yield return ExecuteJump().AsCoroutine();
                    continue;
                }

                yield return TimeYields.WaitOneFrameX;
            }
        }
    }
}