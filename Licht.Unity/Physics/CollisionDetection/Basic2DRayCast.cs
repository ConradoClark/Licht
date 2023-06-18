using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Licht.Unity.Objects;
using UnityEngine;

namespace Licht.Unity.Physics.CollisionDetection
{
    public class Basic2DRayCast : LichtPhysicsCollisionDetector
    {
        public ScriptIdentifier TriggerIdentifier;
        public float Distance;
        public float DistanceToTrigger;

        public bool UseSpeedAsDirection;
        public Vector2 Direction;

        public LayerMask LayerMask;
        private readonly RaycastHit2D[] _collisionResults = new RaycastHit2D[10];

        public bool CheckNormals;
        public Vector2 NormalTarget;

        public bool IncreaseBySpeed;

        public Vector2 Offset;

        protected override void OnAwake()
        {
            base.OnAwake();
            DetectorType = CollisionDetectorType.PreUpdate;
        }

        public override CollisionResult[] CalculateCollision()
        {
            for (var i = 0; i < _collisionResults.Length; i++)
            {
                _collisionResults[i] = default;
            }

            var distance = IncreaseBySpeed
                ? Distance + (PhysicsObject.CalculatedSpeed * Direction).magnitude
                : Distance;

            var noHits = Physics2D.RaycastNonAlloc((Vector2)transform.position + Offset,
                UseSpeedAsDirection ? PhysicsObject.CalculatedSpeed : Direction, _collisionResults,
                distance, LayerMask);

            if (noHits == 0)
            {
                if (TriggerIdentifier != null) PhysicsObject.SetPhysicsTrigger(TriggerIdentifier, false, this);
                return Array.Empty<CollisionResult>();
            }

            var results = _collisionResults
                .Where(col => col.collider != null && !PhysicsObject.CollisionDetectors.Select(c => c.AssociatedCollider).Contains(col.collider)
                                                   && !PhysicsObject.AdditionalColliders.Contains(col.collider)
                && (!CheckNormals || col.normal == NormalTarget)
                )
                .Select(col => new CollisionResult
                {
                    Detected = true,
                    TriggeredHit = col.distance <= DistanceToTrigger,
                    Direction = col.normal,
                    Collider = col.collider,
                    Hit = col,
                })
                .ToArray();

            if (TriggerIdentifier != null) PhysicsObject.SetPhysicsTrigger(TriggerIdentifier, results.Any(t=>t.TriggeredHit), this);
            return results;
        }

        public override Vector2 Clamp()
        {
            if (!ShouldClamp) return PhysicsObject.CalculatedSpeed;
            var results = Triggers;
            if (results == null || results.Length == 0 || PhysicsObject.CalculatedSpeed.magnitude == 0) return PhysicsObject.CalculatedSpeed;

            var targetSpeed = PhysicsObject.CalculatedSpeed;

            foreach (var result in results)
            {
                var distance = result.Hit.point - PhysicsObject.GetCurrentPosition();
                var hasXComponent = Mathf.Abs(Direction.x) > 0;
                var hasYComponent = Mathf.Abs(Direction.y) > 0;

                targetSpeed = new Vector2(Mathf.Clamp(targetSpeed.x,
                        hasXComponent ? -Mathf.Abs(distance.x) : PhysicsObject.CalculatedSpeed.x,
                        hasXComponent ? Mathf.Abs(distance.x) : PhysicsObject.CalculatedSpeed.x),
                    Mathf.Clamp(targetSpeed.y, hasYComponent ? -Mathf.Abs(distance.y) : PhysicsObject.CalculatedSpeed.y,
                        hasYComponent ? Mathf.Abs(distance.y) : PhysicsObject.CalculatedSpeed.y));
            }

            return targetSpeed;
        }
    }
}
