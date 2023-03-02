using System;
using UnityEngine;

namespace Licht.Unity.Objects.Stats
{
    public class ScriptStat<T> : ScriptableObject
    {
        [SerializeField]
        private T _stat;

        public string Name;
        public T Stat
        {
            get => _stat;
            set
            {
                var old = _stat;
                _stat = value;
                OnChange?.Invoke(new StatUpdate
                {
                    OldValue = old,
                    NewValue = value
                });
            }
        }

        public struct StatUpdate
        {
            public T OldValue;
            public T NewValue;
        }

        public event Action<StatUpdate> OnChange;
    }
}
