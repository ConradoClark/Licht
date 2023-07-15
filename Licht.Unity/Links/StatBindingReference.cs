using Licht.Unity.Objects.Stats;
using UnityEngine;

namespace Licht.Unity.Links
{
    public class StatBindingReference<T> : ObservableBindingReference
    {
        [field:SerializeField]
        public ScriptStat<T> Stat { get; protected set; }

        protected override void OnEnable()
        {
            base.OnEnable();
            Stat.OnChange+= OnStatChange;
        }

        private void OnStatChange(ScriptStat<T>.StatUpdate obj)
        {
            OnChange?.Invoke(this);
        }

        public override object Get()
        {
            return Stat.Stat;
        }

        public override void Set(object obj)
        {
            if (obj is T converted)
            {
                Stat.Stat = converted;
            }

            Stat.Stat = (T)obj;
        }

        public override void PreCacheBindings()
        {
        }
    }
}