using UnityEngine;

namespace Licht.Unity.Physics
{
    public struct CollisionResult
    {
        public Vector2 Direction;
        public bool Detected;
        public bool TriggeredHit;
        public Collider2D Collider;
    }
}

