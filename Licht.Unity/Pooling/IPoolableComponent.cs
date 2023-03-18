using System.Collections.Generic;
using Licht.Interfaces.Pooling;
using UnityEngine;

namespace Licht.Unity.Pooling
{
    public interface IPoolableComponent : IPoolableObject
    {
        MonoBehaviour Component { get; }
        Dictionary<string,float> CustomProps { get; }

        Dictionary<string, string> CustomTags { get; }
        bool HasTag(string customTag, string value=null);

        bool HasProp(string customProp, float? value = null, float tolerance = 0.01f);
    }
}
