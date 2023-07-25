using System.Collections.Generic;
using Licht.Interfaces.Pooling;
using Licht.Unity.Objects;
using UnityEngine;

namespace Licht.Unity.Pooling
{
    public interface IPoolableComponent : IPoolableObject
    {
        BaseActor Component { get; }
        void Release();
        bool TryGetCustomObject<T>(out T obj, bool includeParent = true) where T : class;
    }
}
    