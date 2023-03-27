using System.Collections.Generic;
using Licht.Interfaces.Pooling;
using UnityEngine;

namespace Licht.Unity.Pooling
{
    public interface IPoolableComponent : IPoolableObject
    {
        MonoBehaviour Component { get; }
    }
}
