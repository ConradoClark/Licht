using System.Collections.Generic;
using Licht.Interfaces.Pooling;
using Licht.Unity.Objects;
using UnityEngine;

namespace Licht.Unity.Pooling
{
    public interface IPoolableComponent : IPoolableObject
    {
        BaseActor Component { get; }
    }
}
    