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

        public ScriptIdentifier TriggerIdentifier;
        public ScriptIdentifier HitCeilingIdentifier;

        public bool PreventUpClamp;
        public bool PreventDownClamp;
        public bool PreventRightClamp;
        public bool PreventLeftClamp;

        protected override void OnAwake()
        {
            base.OnAwake(); 
            DetectorType = CollisionDetectorType.PostUpdate;
        }

        public override CollisionResult[] CheckCollision()
        {
            var noHits = Collider.OverlapCollider(ContactFilter, _collisionResults);
            if (noHits == 0)
            {
                if (TriggerIdentifier != null) PhysicsObject.SetPhysicsTrigger(TriggerIdentifier, false, this);
                return Array.Empty<CollisionResult>();
            }

            var results = _collisionResults
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

            if (TriggerIdentifier != null) PhysicsObject.SetPhysicsTrigger(TriggerIdentifier, results.Any(r=>r.TriggeredHit), this);
            return results;
        }

        public override Vector2 Clamp()
        {
            if (HitCeilingIdentifier != null)
            {
                PhysicsObject.SetPhysicsTrigger(HitCeilingIdentifier, false, this);
            }

            if (!ShouldClamp) return PhysicsObject.transform.position;
            var results = Triggers;
            if (results == null || results.Length == 0 ||
                (PhysicsObject.CalculatedSpeed.magnitude == 0 && results.All(r=> !Collider.Distance(r.Collider).isOverlapped))) return PhysicsObject.transform.position;
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
                                                                              && Vector2.Angle(distance.normal, Vector2.up) < 90, this);
                    }

                    var clamp = new Vector3(
                        Mathf.Clamp(distanceToCollide.x, PreventLeftClamp ? 0 : distanceToCollide.x,
                            PreventRightClamp ? 0 : distanceToCollide.x),
                        Mathf.Clamp(distanceToCollide.y, PreventDownClamp ? 0 : distanceToCollide.y,
                            PreventUpClamp ? 0 : distanceToCollide.y)
                    );

                    clampedPosition += clamp;
                }
            }
            return clampedPosition;
        }
    }
}
