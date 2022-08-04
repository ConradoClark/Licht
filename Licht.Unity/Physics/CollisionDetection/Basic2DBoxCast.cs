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

        public bool IncreaseBySpeed;
        public bool ShouldClampSpeed;

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

            var noHits = Physics2D.BoxCastNonAlloc((Vector2)BoxCollider.transform.position + BoxCollider.offset, BoxCollider.size, 0, Direction, _collisionResults,
                IncreaseBySpeed ? Distance + (PhysicsObject.CalculatedSpeed * Direction).magnitude : Distance, LayerMask);
            if (noHits == 0)
            {
                if (TriggerIdentifier != null) PhysicsObject.SetPhysicsTrigger(TriggerIdentifier, false, this);
                return Array.Empty<CollisionResult>();
            }

            var results = _collisionResults
                .Where(col => col.collider != null && !PhysicsObject.CollisionDetectors.Select(c => c.Collider).Contains(col.collider)
                                                   && !PhysicsObject.AdditionalColliders.Contains(col.collider)
                && (!CheckNormals || col.normal == NormalTarget)
                )
                .Select(col => new CollisionResult
                {
                    Detected = true,
                    TriggeredHit = col.distance <= Distance,
                    Direction = col.normal,
                    Collider = col.collider,
                    Hit = col,
                })
                .ToArray();

            if (TriggerIdentifier != null) PhysicsObject.SetPhysicsTrigger(TriggerIdentifier, results.Any(), this);
            return results;
        }

        public override Vector2 Clamp()
        {
            if (!ShouldClampSpeed) return PhysicsObject.CalculatedSpeed;
            var results = Triggers;
            if (results == null || results.Length == 0 || PhysicsObject.CalculatedSpeed.magnitude == 0) return PhysicsObject.CalculatedSpeed;

            var targetSpeed = PhysicsObject.CalculatedSpeed;

            foreach (var result in results)
            {
                var distance = result.Hit.point - (Vector2) PhysicsObject.transform.position;
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
