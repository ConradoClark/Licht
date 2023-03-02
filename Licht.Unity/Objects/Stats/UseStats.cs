using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Licht.Unity.Objects.Stats
{
    [CreateAssetMenu(fileName = "ObjectStats", menuName = "Licht/Stats/ObjectStats", order = 1)]
    public class ObjectStats : ScriptableObject
    {
        public ScriptIntegerStat[] IntStats;
        public ScriptFloatStat[] FloatStats;
        public ScriptStringStat[] StringStats;
    }
}
