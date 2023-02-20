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

        [Serializable]
        public class CustomJumpParams
        {
            public float JumpSpeed;
            public float AccelerationTime;
            public float DecelerationTime;
            public EasingYields.EasingFunction MovementStartEasing;
            public EasingYields.EasingFunction MovementEndEasing;
            public float? MinJumpDelay;
        }

        public struct LichtPlatformerJumpEventArgs
        {
            public LichtPlatformerJumpController Source;
            public CustomJumpParams CustomParams;
        }

        public ScriptIdentifier GroundedTrigger;
        public ScriptIdentifier CeilingTrigger;
        public ScriptIdentifier GravityIdentifier;
        public InputActionReference JumpInput;
        public float JumpSpeed;
        public float AccelerationTime;
        public float DecelerationTime;
        public EasingYields.EasingFunction MovementStartEasing;
        public EasingYields.EasingFunction MovementEndEasing;
        public LichtPhysicsObject Target;
        public float InputBufferTime;
        public float CoyoteJumpTime;

        public float MinJumpHoldInSeconds;

        private bool _minJumpDelayPassed;

        public bool IsJumping { get; protected set; }
        public bool Interrupted { get; protected set; }

        public bool IsForcedJump { get; protected set; }

        private LichtPhysics _physics;
        private PlayerInput _input;
        private IEventPublisher<LichtPlatformerJumpEvents, LichtPlatformerJumpEventArgs> _eventPublisher;

        protected override void OnAwake()
        {
            base.OnAwake();
            _physics = this.GetLichtPhysics();
            _input = PlayerInput.GetPlayerByIndex(0);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _physics.LichtPhysicsMachinery.Machinery.AddBasicMachine(HandleJump());
            _eventPublisher = this.RegisterAsEventPublisher<LichtPlatformerJumpEvents, LichtPlatformerJumpEventArgs>();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            this.UnregisterAsEventPublisher<LichtPlatformerJumpEvents, LichtPlatformerJumpEventArgs>();
        }

        public void Interrupt()
        {
            if (IsJumping) Interrupted = true;
        }

        private IEnumerable<IEnumerable<Action>> WaitMinJumpDelay(float? minDelay = null)
        {
            yield return TimeYields.WaitSeconds(GameTimer, minDelay ?? MinJumpHoldInSeconds);
            _minJumpDelayPassed = true;
        }

        public IEnumerable<IEnumerable<Action>> ExecuteJump(InputAction jumpAction = null, CustomJumpParams customParams = null
            , bool forced = true)
        {
            IsForcedJump = forced;
            jumpAction ??= _input.actions[JumpInput.action.name];

            IsJumping = true;
            _eventPublisher.PublishEvent(LichtPlatformerJumpEvents.OnJumpStart, new LichtPlatformerJumpEventArgs
            {
                Source = this,
                CustomParams = customParams
            });

            _physics.BlockCustomPhysicsForceForObject(this, Target, GravityIdentifier.Name);
            yield return TimeYields.WaitOneFrameX;

            _minJumpDelayPassed = false;

            DefaultMachinery.AddBasicMachine(WaitMinJumpDelay(customParams?.MinJumpDelay));

            yield return Target.GetSpeedAccessor()
                .Y
                .SetTarget(customParams?.JumpSpeed ?? JumpSpeed)
                .Over(customParams?.AccelerationTime ?? AccelerationTime)
                .Easing(customParams?.MovementStartEasing ?? MovementStartEasing)
                .BreakIf(() => (!jumpAction.IsPressed() && _minJumpDelayPassed) ||
                               Target.GetPhysicsTrigger(CeilingTrigger) || IsBlocked || Interrupted, false)
                .UsingTimer(_physics.ScriptTimerRef.Timer)
                .Build();

            yield return Target.GetSpeedAccessor(new Vector2(0, Target.LatestSpeed.y))
                .Y
                .SetTarget(0)
                .Over(customParams?.DecelerationTime ?? DecelerationTime)
                .Easing(customParams?.MovementEndEasing ?? MovementEndEasing)
                .BreakIf(() => (!jumpAction.IsPressed() && _minJumpDelayPassed) ||
                               Target.GetPhysicsTrigger(CeilingTrigger) || IsBlocked || Interrupted, false)
                .UsingTimer(_physics.ScriptTimerRef.Timer)
                .Build();

            yield return TimeYields.WaitOneFrameX;

            Interrupted = false;
            IsJumping = false;
            IsForcedJump = false;
            _physics.UnblockCustomPhysicsForceForObject(this, Target, GravityIdentifier.Name);
            _eventPublisher.PublishEvent(LichtPlatformerJumpEvents.OnJumpEnd, new LichtPlatformerJumpEventArgs
            {
                Source = this
            });
        }

        private bool IsGrounded()
        {
            return Target.GetPhysicsTrigger(GroundedTrigger);
        }


        protected virtual IEnumerable<IEnumerable<Action>> HandleJump()
        {
            var jumpInput = _input.actions[JumpInput.action.name];
            while (isActiveAndEnabled)
            {
                var jumped = false;
                while (!IsGrounded())
                {
                    if (!isActiveAndEnabled)
                    {
                        yield break;
                    }

                    if (IsBlocked)
                    {
                        yield return TimeYields.WaitOneFrameX;
                        continue;
                    }

                    if (InputBufferTime > 0 && jumpInput.WasPerformedThisFrame())
                    {
                        foreach (var _ in TimeYields.WaitSeconds(_physics.ScriptTimerRef.Timer, InputBufferTime))
                        {
                            if (!IsGrounded() || IsBlocked || IsJumping)
                            {
                                yield return TimeYields.WaitOneFrameX;
                                continue;
                            }

                            jumped = true;
                            yield return ExecuteJump(jumpInput, forced:false).AsCoroutine();
                            break;
                        }
                    }
                    else
                    {
                        yield return TimeYields.WaitOneFrameX;
                    }

                    if (jumped) continue;
                }

                if (CoyoteJumpTime > 0 && !jumped)
                {
                    foreach (var _ in TimeYields.WaitSeconds(_physics.ScriptTimerRef.Timer, CoyoteJumpTime))
                    {
                        if (!jumpInput.WasPerformedThisFrame() || IsBlocked || IsJumping)
                        {
                            yield return TimeYields.WaitOneFrameX;
                            continue;
                        }

                        jumped = true;
                        yield return ExecuteJump(jumpInput, forced: false).AsCoroutine();
                        break;
                    }
                }

                if (jumpInput.WasPerformedThisFrame() && !IsBlocked && !jumped && !IsJumping)
                {
                    yield return ExecuteJump(jumpInput, forced: false).AsCoroutine();
                    continue;
                }

                yield return TimeYields.WaitOneFrameX;
            }
        }
    }
}