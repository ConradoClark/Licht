using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Licht.Unity.Objects.Stats
{
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


        private void Awake()
        {
            _intStatsDict =
                new Dictionary<string, ScriptStat<int>>(IntStats?.Select(i =>
                    new KeyValuePair<string, ScriptStat<int>>(i.Name, i)));

            _floatStatsDict =
                new Dictionary<string, ScriptStat<float>>(FloatStats?.Select(i =>
                    new KeyValuePair<string, ScriptStat<float>>(i.Name, i)));

            _stringStatsDict =
                new Dictionary<string, ScriptStat<string>>(StringStats?.Select(i =>
                    new KeyValuePair<string, ScriptStat<string>>(i.Name, i)));

            Ints = new DictAccessor<int>(_intStatsDict);
            Floats = new DictAccessor<float>(_floatStatsDict);
            Strings = new DictAccessor<string>(_stringStatsDict);
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
                get => _dict[index].Stat;
                set => _dict[index].Stat = value;
            }
        }
    }
}
