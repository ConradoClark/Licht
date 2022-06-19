using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

namespace Licht.Unity.UI.Options
{
    public class UIChangeVolumeKnob : UIAction
    {
        public AudioSource TestAudio;
        public AudioMixerGroup Mixer;
        public SpriteRenderer Gauge;
        public SpriteRenderer ShadowGauge;

        public PlayerInput PlayerInput;
        public ScriptInput KnobControl;

        public string VolumeParam;
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
            ShadowGauge.size = new Vector2((VolumeLevels ?? DefaultVolumeLevels).Keys.Max() * GaugeLevelSize,
                ShadowGauge.size.y);
            DefaultMachinery.AddBasicMachine(HandleKnob());
        }

        private IEnumerable<IEnumerable<Action>> HandleKnob()
        {
            var action = PlayerInput.actions[KnobControl.ActionName];

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
