using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Licht.Unity.Physics.CollisionDetection
{
    public class Basic2DCollisionDetector : LichtPhysicsCollisionDetector
    {
        public ContactFilter2D ContactFilter;
        private readonly List<Collider2D> _collisionResults = new List<Collider2D>();

        public override CollisionDetectorType DetectorType { get; protected set; } = CollisionDetectorType.PostUpdate;

        public override CollisionResult[] CheckCollision()
        {
            var noHits = Collider.OverlapCollider(ContactFilter, _collisionResults);
            if (noHits == 0) return Array.Empty<CollisionResult>();

            return _collisionResults
                .Where(col => !PhysicsObject.CollisionDetectors.Select(c => c.Collider).Contains(col))
                .Select(col =>
                {
                    var colliderDistance = col.Distance(Collider);

                    return new CollisionResult
                    {
                        Detected = true,
                        TriggeredHit = colliderDistance.isOverlapped,
                        Direction = colliderDistance.normal,
                        Collider = col,
                    };
                })
                .ToArray();
        }

        public override Vector2 Clamp()
        {
            var results = Triggers;
            if (results == null || results.Length == 0 || PhysicsObject.CalculatedSpeed.magnitude == 0) return Collider.transform.position;
            var clampedPosition = Collider.transform.position;

            foreach (var result in results)
            {
                var distance = Collider.Distance(result.Collider);
                var distanceToCollide = distance.pointB - distance.pointA;

                if (distance.isOverlapped)
                {
                    clampedPosition += (Vector3)distanceToCollide;
                }
            }
            return clampedPosition;
        }
    }
}
