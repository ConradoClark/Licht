using UnityEngine;

namespace Licht.Unity.Physics
{
    public struct CollisionResult
    {
        public CollisionOrientation Orientation;
        public Vector2 Direction;
        public RaycastHit2D[] Hits;
        public RaycastHit2D ClosestHit;
        public bool Detected;
        public bool TriggeredHit;
        public float OriginalDistance;
        public float TravelDistance;
        public Vector2 ClampPoint;
    }
}

