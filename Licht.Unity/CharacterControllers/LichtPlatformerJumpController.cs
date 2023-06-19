using System;
using System.Collections.Generic;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using Licht.Unity.PropertyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Licht.Unity.CharacterControllers
{
    [AddComponentMenu("L. Controllers: Platformer2D Jump")]
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
            public string Identifier;
        }

        public struct LichtPlatformerJumpEventArgs
        {
            public LichtPlatformerJumpController Source;
            public CustomJumpParams CustomParams;
        }

        [Serializable]
        public struct LichtPlatformerJumpControllerUnityEvents
        {
            public UnityEvent<LichtPlatformerJumpEventArgs> OnJumpStart;
            public UnityEvent<LichtPlatformerJumpEventArgs> OnJumpEnd;
        }

        [CustomHeader("Movement")]
        [BeginFoldout("Triggers")]
        [CustomLabel("Triggered when the actor hits the ground.")]
        public ScriptIdentifier GroundedTrigger;
        [CustomLabel("Triggered when the actor hits the ceiling.")]
        public ScriptIdentifier CeilingTrigger;
        [CustomLabel("Gravity identifier in the Physics System.")]
        public ScriptIdentifier GravityIdentifier;
        [EndFoldout]
        [BeginFoldout("Movement Properties")]
        public float JumpSpeed;
        [InspectorName("Acceleration Time (s)")]
        public float AccelerationTime;
        [InspectorName("Deceleration Time (s)")]
        public float DecelerationTime;
        public EasingYields.EasingFunction MovementStartEasing;
        public EasingYields.EasingFunction MovementEndEasing;
        [EndFoldout]
        [BeginFoldout("Tweaks")]
        [CustomLabel("Input buffering when not on ground.")]
        [InspectorName("Input Buffer Time (s)")]
        public float InputBufferTime;
        [CustomLabel("Jump leniency buffer when leaving ground.")]
        [InspectorName("Coyote Jump Time (s)")]
        public float CoyoteJumpTime;
        [CustomLabel("Min time the jump is automatically held, in seconds.")]
        [InspectorName("Minimum Jump Hold (s)")]
        public float MinJumpHoldInSeconds;
        [EndFoldout]
        [CustomHeader("Reference")]
        [CustomLabel("The default is the associated Actor.")]
        [CustomLabel("Select this if you want to attach this to a different Physics Object.")]
        public bool UseCustomTarget;
        [ShowWhen(nameof(UseCustomTarget))]
        public LichtPhysicsObject Target;
        [CustomHeader("Input")]
        public InputActionReference JumpInput;
        [field:CustomHeader("Events")]
        [field:SerializeField]
        public LichtPlatformerJumpControllerUnityEvents UnityEvents { get; private set; }

        private bool _minJumpDelayPassed;

        public bool IsJumping { get; protected set; }
        public bool Interrupted { get; protected set; }

        public bool IsForcedJump { get; protected set; }

        private LichtPhysics _physics;
        private PlayerInput _input;
        private IEventPublisher<LichtPlatformerJumpEvents, LichtPlatformerJumpEventArgs> _eventPublisher;

        public event Action<LichtPlatformerJumpEventArgs> OnJumpStart;
        public event Action<LichtPlatformerJumpEventArgs> OnJumpEnd;

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
            _minJumpDelayPassed = false;
            yield return TimeYields.WaitSeconds(GameTimer, minDelay ?? MinJumpHoldInSeconds,
                breakCondition: () => !IsJumping);
            _minJumpDelayPassed = true;
            Target.Sticky = true;
        }

        public IEnumerable<IEnumerable<Action>> ExecuteJump(InputAction jumpAction = null, CustomJumpParams customParams = null
            , bool forced = true)
        {
            if (IsJumping) yield break;
            IsForcedJump = forced;
            jumpAction ??= _input.actions[JumpInput.action.name];
            Target.Sticky = false;

            IsJumping = true;
            var eventObject = new LichtPlatformerJumpEventArgs
            {
                Source = this,
                CustomParams = customParams
            };

            _eventPublisher.PublishEvent(LichtPlatformerJumpEvents.OnJumpStart, eventObject);
            OnJumpStart?.Invoke(eventObject);
            UnityEvents.OnJumpStart?.Invoke(eventObject);

            _physics.BlockCustomPhysicsForceForObject(this, Target, GravityIdentifier.name);
            yield return TimeYields.WaitOneFrameX;

            DefaultMachinery.AddUniqueMachine($"minJumpDelay_{gameObject.GetInstanceID()}", UniqueMachine.UniqueMachineBehaviour.Replace,
                WaitMinJumpDelay(customParams?.MinJumpDelay));

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

            IsJumping = false;

            yield return TimeYields.WaitOneFrameX;

            Interrupted = false;
            IsForcedJump = false;
            _physics.UnblockCustomPhysicsForceForObject(this, Target, GravityIdentifier.name);
            _eventPublisher.PublishEvent(LichtPlatformerJumpEvents.OnJumpEnd, eventObject);
            OnJumpEnd?.Invoke(eventObject);
            UnityEvents.OnJumpEnd?.Invoke(eventObject);
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
                            if (!IsGrounded() || IsBlocked)
                            {
                                yield return TimeYields.WaitOneFrameX;
                                continue;
                            }

                            jumped = true;
                            yield return ExecuteJump(jumpInput, forced: false).AsCoroutine();
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