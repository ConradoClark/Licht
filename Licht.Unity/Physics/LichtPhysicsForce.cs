using System;
using System.Collections.Generic;
using UnityEngine;

namespace Licht.Unity.Physics
{
    [Serializable]
    public struct LichtPhysicsForce
    {
        public Vector2Int AxisLock;
        public Vector2 Speed;
        public bool Active;
        public bool Temporary;
        public LayerMask AffectsObjectsByLayer;
        public List<LichtPhysicsObject> AffectsObjects;
    }
}

