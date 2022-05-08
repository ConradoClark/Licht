using UnityEngine;

namespace Licht.Unity.Physics
{
    public struct CollisionResult
    {
        public CollisionOrientation Orientation;
        public RaycastHit2D[] Hits;
        public bool TriggeredHit;
        public bool HitNegative;
        public bool HitPositive;
    }
}

