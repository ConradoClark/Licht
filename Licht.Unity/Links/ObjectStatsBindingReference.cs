using System;
using Licht.Unity.Objects.Stats;
using UnityEngine;

namespace Licht.Unity.Links
{
    [AddComponentMenu("L. Link: Object Stats Binding")]
    public class ObjectStatsBindingReference : ObservableBindingReference
    {
        [field: SerializeField] public ObjectStats Stats { get; private set; }

        [field: SerializeField] public string StatName { get; private set; }

        private Func<object> _getter;
        private Action<object> _setter;

        protected override void OnEnable()
        {
            base.OnEnable();
            _getter = Stats.Ints.GetStat(StatName)?.Name != null ? () => Stats.Ints[StatName]
                : Stats.Floats.GetStat(StatName)?.Name != null ? () => Stats.Floats[StatName]
                : Stats.Strings.GetStat(StatName)?.Name != null ? () => Stats.Strings[StatName]
                : new Func<object>(() => null);

            _setter = Stats.Ints.GetStat(StatName)?.Name != null ? value => Stats.Ints[StatName] = (int)value
                : Stats.Floats.GetStat(StatName)?.Name != null ? value => Stats.Floats[StatName] = (float)value
                : Stats.Strings.GetStat(StatName)?.Name != null ? value => Stats.Strings[StatName] = value?.ToString()
                : new Action<object>(value => { });
        }

        public override object Get()
        {
            return _getter();
        }

        public override void Set(object obj)
        {
            _setter(obj);
            OnChange.Invoke(this);
        }

        public override void PreCacheBindings()
        {
        }
    }
}