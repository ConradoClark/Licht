using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Licht.Unity.Objects.Stats
{
    [DefaultExecutionOrder(-2000)]
    [CreateAssetMenu(fileName = "ObjectStats", menuName = "Licht/Stats/ObjectStats", order = 1)]
    public class ObjectStats : ScriptableObject
    {
        [field:SerializeField]
        public ScriptIntegerStat[] IntStats { get; private set; }
        [field: SerializeField]
        public ScriptFloatStat[] FloatStats { get; private set; }
        [field: SerializeField]
        public ScriptStringStat[] StringStats { get; private set; }

        private Dictionary<string, ScriptStat<int>> _intStatsDict;
        private Dictionary<string, ScriptStat<float>> _floatStatsDict;
        private Dictionary<string, ScriptStat<string>> _stringStatsDict;

        public DictAccessor<int> Ints { get; private set; }
        public DictAccessor<float> Floats { get; private set; }
        public DictAccessor<string> Strings { get; private set; }


        private void OnEnable()
        {
            if (IntStats != null)
            {
                _intStatsDict =
                    new Dictionary<string, ScriptStat<int>>(IntStats?.Select(i =>
                        new KeyValuePair<string, ScriptStat<int>>(i.Name, i)));
            }

            if (FloatStats != null)
            {
                _floatStatsDict =
                    new Dictionary<string, ScriptStat<float>>(FloatStats?.Select(i =>
                        new KeyValuePair<string, ScriptStat<float>>(i.Name, i)));
            }

            if (StringStats != null)
            {
                _stringStatsDict =
                    new Dictionary<string, ScriptStat<string>>(StringStats?.Select(i =>
                        new KeyValuePair<string, ScriptStat<string>>(i.Name, i)));
            }

            Ints = new DictAccessor<int>(_intStatsDict ?? new Dictionary<string, ScriptStat<int>>());
            Floats = new DictAccessor<float>(_floatStatsDict ?? new Dictionary<string, ScriptStat<float>>());
            Strings = new DictAccessor<string>(_stringStatsDict ?? new Dictionary<string, ScriptStat<string>>());
        }

        public class DictAccessor<T>
        {
            private readonly IDictionary<string, ScriptStat<T>> _dict;

            public DictAccessor(IDictionary<string, ScriptStat<T>> dict)
            {
                _dict = dict;
            }
            public T this[string index]
            {
                get => _dict.ContainsKey(index) ? _dict [index].Stat : default;
                set => _dict[index].Stat = value;
            }

            public ScriptStat<T> GetStat(string index)
            {
                return _dict.ContainsKey(index) ? _dict[index] : default;
            }
        }
    }
}
