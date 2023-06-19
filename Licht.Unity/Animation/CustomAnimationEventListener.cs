using System;
using System.Collections.Generic;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;
using Licht.Unity.Objects;
using Licht.Unity.PropertyAttributes;
using UnityEngine;

namespace Licht.Unity.Animation
{
    [AddComponentMenu("L!> Animation: Event Listener")]
    public class CustomAnimationEventListener : BaseGameObject
    {
        [Serializable]
        public struct CustomAnimationEvent
        {
            [CustomLabel("Animation Layer associated to the event.")]
            public int AnimationLayerIndex;
            [CustomLabel("Animator State associated to the event.")]
            public string StateName;
            public string EventName;

            [CustomLabel("Events are fired once per range.")]
            [CustomLabel("Min keyframe to trigger the event.")]
            public int KeyFrameMin;
            [CustomLabel("Max keyframe to trigger the event.")]
            public int KeyFrameMax;

            public override string ToString()
            {
                return $"{StateName} - {EventName} on frames {KeyFrameMin}-{KeyFrameMax}";
            }
        }

        public struct CustomAnimationEventHandler
        {
            public CustomAnimationEvent AnimEvent;
            public CustomAnimationEventListener Source;
        }

        public enum AnimationEventType
        {
            OnCustomAnimationEvent,
            OnCustomAnimationEventExit
        }

        public Animator Animator;
        public CustomAnimationEvent[] AnimationEvents;

        private IEventPublisher<AnimationEventType, CustomAnimationEventHandler> _eventPublisher;
        private bool _enabled;
        private HashSet<string> _blockedEvents;
        public int AnimationLayer;


        protected override void OnEnable()
        {
            base.OnEnable();
            _enabled = true;
            _blockedEvents = new HashSet<string>();
            _eventPublisher = this.RegisterAsEventPublisher<AnimationEventType, CustomAnimationEventHandler>();
            DefaultMachinery.AddBasicMachine(Execute());
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _enabled = false;
            this.UnregisterAsEventPublisher<AnimationEventType, CustomAnimationEventHandler>();
        }

        private IEnumerable<IEnumerable<Action>> Execute()
        {
            while (_enabled)
            {
                foreach (var anim in AnimationEvents)
                {
                    if (_blockedEvents.Contains(anim.EventName)) continue;
                    var stateInfo = Animator.GetCurrentAnimatorStateInfo(anim.AnimationLayerIndex);
                    if (!stateInfo.IsName(anim.StateName))
                    {
                        continue;
                    }

                    var clipInfo = Animator.GetCurrentAnimatorClipInfo(AnimationLayer);
                    if (clipInfo.Length < 1) continue;

                    var frameRate = clipInfo[0].clip.frameRate;
                    var length = clipInfo[0].clip.length;
                    var currentFrame = (int)(stateInfo.normalizedTime * length * frameRate % (length * frameRate));

                    if (currentFrame > anim.KeyFrameMax || currentFrame < anim.KeyFrameMin) continue;

                    _eventPublisher.PublishEvent(AnimationEventType.OnCustomAnimationEvent, new CustomAnimationEventHandler()
                    {
                        AnimEvent = anim,
                        Source = this
                    });
                    _blockedEvents.Add(anim.EventName);
                    DefaultMachinery.AddBasicMachine(HandleBlockedEvent(anim));
                }
                yield return TimeYields.WaitOneFrameX;
            }
        }

        private IEnumerable<IEnumerable<Action>> HandleBlockedEvent(CustomAnimationEvent anim)
        {
            AnimatorStateInfo stateInfo;
            int currentFrame;

            do
            {
                yield return TimeYields.WaitOneFrameX;

                stateInfo = Animator.GetCurrentAnimatorStateInfo(anim.AnimationLayerIndex);
                var clipInfo = Animator.GetCurrentAnimatorClipInfo(AnimationLayer);
                if (clipInfo.Length < 1) break;
                var frameRate = clipInfo[0].clip.frameRate;
                var length = clipInfo[0].clip.length;
                currentFrame = (int)(stateInfo.normalizedTime * length * frameRate % (length * frameRate));
            } while (_enabled && currentFrame >= anim.KeyFrameMin && currentFrame <= anim.KeyFrameMax && stateInfo.IsName(anim.StateName));

            _eventPublisher.PublishEvent(AnimationEventType.OnCustomAnimationEventExit, new CustomAnimationEventHandler()
            {
                AnimEvent = anim,
                Source = this
            });

            _blockedEvents.Remove(anim.EventName);
        }
    }
}
