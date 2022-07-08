using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Unity.Objects;
using UnityEngine;

namespace Licht.Unity.Physics.CollisionDetection
{
    public class Basic2DCollisionDetector : LichtPhysicsCollisionDetector
    {
        public bool ShouldClamp;
        public ContactFilter2D ContactFilter;
        private readonly List<Collider2D> _collisionResults = new List<Collider2D>();

        public ScriptIdentifier HitCeilingIdentifier;

        protected override void OnAwake()
        {
            base.OnAwake();
            DetectorType = CollisionDetectorType.PostUpdate;
        }

        public override CollisionResult[] CheckCollision()
        {
            var noHits = Collider.OverlapCollider(ContactFilter, _collisionResults);
            if (noHits == 0) return Array.Empty<CollisionResult>();

            return _collisionResults
                .Where(col => !PhysicsObject.CollisionDetectors.Select(c => c.Collider).Contains(col)
                   && !PhysicsObject.AdditionalColliders.Contains(col))
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
            if (HitCeilingIdentifier != null)
            {
                PhysicsObject.SetPhysicsTrigger(HitCeilingIdentifier, false);
            }

            if (!ShouldClamp) return PhysicsObject.transform.position;
            var results = Triggers;
            if (results == null || results.Length == 0 || PhysicsObject.CalculatedSpeed.magnitude == 0) return PhysicsObject.transform.position;
            var clampedPosition = PhysicsObject.transform.position;

            foreach (var result in results)
            {
                var distance = Collider.Distance(result.Collider);
                var distanceToCollide = distance.pointB - distance.pointA;

                if (distance.isOverlapped)
                {
                    if (HitCeilingIdentifier != null)
                    {
                        PhysicsObject.SetPhysicsTrigger(HitCeilingIdentifier, distance.pointA.y > PhysicsObject.transform.position.y
                                                                              && Vector2.Angle(distance.normal, Vector2.up) < 90);
                    }

                    clampedPosition += (Vector3)distanceToCollide;
                }
            }
            return clampedPosition;
        }
    }
}
