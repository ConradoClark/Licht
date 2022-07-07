using System;
using System.Linq;
using Licht.Unity.Objects;
using UnityEngine;

namespace Licht.Unity.Physics.CollisionDetection
{
    public class Basic2DBoxCast : LichtPhysicsCollisionDetector
    {
        public ScriptIdentifier TriggerIdentifier;
        public float Distance;
        public Vector2 Direction;
        public LayerMask LayerMask;
        private readonly RaycastHit2D[] _collisionResults = new RaycastHit2D[10];

        public bool CheckNormals;
        public Vector2 NormalTarget;

        public BoxCollider2D BoxCollider;
        protected override void OnAwake()
        {
            base.OnAwake();
            Collider = BoxCollider;
            DetectorType = CollisionDetectorType.PreUpdate;
        }

        public override CollisionResult[] CheckCollision()
        {
            for (var i = 0; i < _collisionResults.Length; i++)
            {
                _collisionResults[i] = default;
            }

            var noHits = Physics2D.BoxCastNonAlloc((Vector2) BoxCollider.transform.position + BoxCollider.offset, BoxCollider.size, 0, Direction, _collisionResults, Distance, LayerMask);
            if (noHits == 0)
            {
                PhysicsObject.SetPhysicsTrigger(TriggerIdentifier, false);
                return Array.Empty<CollisionResult>();
            }

            var results = _collisionResults
                .Where(col => col.collider != null && !PhysicsObject.CollisionDetectors.Select(c=>c.Collider).Contains(col.collider)
                && (!CheckNormals || col.normal == NormalTarget)
                )
                .Select(col => new CollisionResult
                {
                    Detected = true,
                    TriggeredHit = true,
                    Direction = col.normal,
                    Collider = col.collider
                })
                .ToArray();

            PhysicsObject.SetPhysicsTrigger(TriggerIdentifier, results.Any());
            return results;
        }

        public override Vector2 Clamp()
        {
            return PhysicsObject.CalculatedSpeed;
            
            
            // fix this later
            //var results = Triggers;
            //if (results == null || results.Length == 0 || obj.CalculatedSpeed.magnitude == 0) return Collider.transform.position;
            //var clampedPosition = Collider.transform.position;

            //foreach (var result in results)
            //{
            //    var distance = Collider.Distance(result.Collider);
            //    var distanceToCollide = distance.pointB - distance.pointA;

            //    if (distance.isOverlapped)
            //    {
            //        clampedPosition += (Vector3)distanceToCollide;
            //    }
            //}
            //return clampedPosition;
        }
    }
}
