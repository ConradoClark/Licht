using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using Licht.Unity.PropertyAttributes;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

namespace Licht.Unity.UI.Options
{
    [AddComponentMenu("L. UI Action: Volume Knob")]
    public class UIChangeVolumeKnob : UIAction
    {
        [BeginFoldout("Audio Settings")]
        public AudioSource TestAudio;
        public AudioMixerGroup Mixer;
        public string VolumeParam;
        [EndFoldout]
        [BeginFoldout("Input")]
        [CustomLabel("Select this if you need to specify a particular PlayerInput")]
        public bool UseCustomPlayerInput;
        [ShowWhen(nameof(UseCustomPlayerInput))]
        public PlayerInput PlayerInput;
        [CustomLabel("Input Axis to control the knob")]
        public InputActionReference AxisKnobControl;
        [EndFoldout]
        [BeginFoldout("Visuals")]
        [CustomLabel("A tiled sprite representing the volume gauge.")]
        public SpriteRenderer Gauge;
        [CustomLabel("A tiled sprite representing the border of the volume gauge.")]
        public SpriteRenderer ShadowGauge;
        [CustomLabel("The volume gauge size at max volume.")]
        public float GaugeLevelSize;
        public int SelectedLevel { get; private set; }

        public virtual Dictionary<int, float> VolumeLevels { get; }

        private static readonly Dictionary<int, float> DefaultVolumeLevels = new Dictionary<int, float>
        {
            { 0, -80f },
            { 1, -30f },
            { 2, -25f },
            { 3, -18.7f },
            { 4, -11.2f },
            { 5, -9f },
            { 6, -4.9f },
            { 7, 0 },
            { 8, 4.5f },
            { 9, 8.3f },
        };

        public override IEnumerable<IEnumerable<Action>> DoAction()
        {
            if (TestAudio != null) TestAudio.Play();
            yield break;
        }

        public override void OnSelect(bool manual)
        {
            
        }

        public override void OnDeselect()
        {
        }

        public override void OnInit()
        {
            if (!UseCustomPlayerInput) PlayerInput = PlayerInput.GetPlayerByIndex(0);
            ShadowGauge.size = new Vector2((VolumeLevels ?? DefaultVolumeLevels).Keys.Max() * GaugeLevelSize,
                ShadowGauge.size.y);
            DefaultMachinery.AddBasicMachine(HandleKnob());
        }

        private IEnumerable<IEnumerable<Action>> HandleKnob()
        {
            var action = PlayerInput.actions[AxisKnobControl.action.name];

            while (isActiveAndEnabled)
            {
                if (action.WasPerformedThisFrame() && Selected)
                {
                    if (TestAudio != null) TestAudio.Play();
                    var sign = Mathf.RoundToInt(Mathf.Sign(action.ReadValue<float>()));
                    SelectedLevel += sign;
                    SelectedLevel = Mathf.Clamp(SelectedLevel, 0, 9);
                    Gauge.size = new Vector2(GaugeLevelSize * SelectedLevel, Gauge.size.y);
                    Mixer.audioMixer.SetFloat(VolumeParam, (VolumeLevels ?? DefaultVolumeLevels)[SelectedLevel]);
                }

                yield return TimeYields.WaitOneFrameX;
            }
        }
    }
}
