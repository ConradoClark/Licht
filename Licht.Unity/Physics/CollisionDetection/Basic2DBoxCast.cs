using System;
using System.Linq;
using Licht.Unity.Objects;
using UnityEngine;

namespace Licht.Unity.Physics.CollisionDetection
{
    public class Basic2DBoxCast : LichtPhysicsCollisionDetector
    {
        public bool Debug;
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

            var distance = IncreaseBySpeed
                ? Distance + (PhysicsObject.CalculatedSpeed * Direction).magnitude
                : Distance;

            var colliderPosition = BoxCollider.transform.position + (Vector3) BoxCollider.offset;

            var noHits = Physics2D.BoxCastNonAlloc((Vector2)colliderPosition, BoxCollider.size, 
                0, Direction, _collisionResults, distance, LayerMask);

            if (Debug)
            {
                var topLeft = new Vector3(BoxCollider.size.x * -0.5f, BoxCollider.size.y * 0.5f);
                var topRight = new Vector3(BoxCollider.size.x * 0.5f, BoxCollider.size.y * 0.5f);
                var bottomLeft = new Vector3(BoxCollider.size.x * -0.5f, BoxCollider.size.y * -0.5f);
                var bottomRight = new Vector3(BoxCollider.size.x * 0.5f, BoxCollider.size.y * -0.5f);
                UnityEngine.Debug.DrawLine(colliderPosition + topLeft, colliderPosition + topRight, Color.green);
                UnityEngine.Debug.DrawLine(colliderPosition + topRight, colliderPosition + bottomRight, Color.green);
                UnityEngine.Debug.DrawLine(colliderPosition + bottomRight, colliderPosition + bottomLeft, Color.green);
                UnityEngine.Debug.DrawLine(colliderPosition + bottomLeft, colliderPosition + topLeft, Color.green);

                var topLeftCast = topLeft + (Vector3)(distance * Direction);
                var topRightCast = topRight + (Vector3)(distance * Direction);
                var bottomLeftCast = bottomLeft + (Vector3)(distance * Direction);
                var bottomRightCast = bottomRight + (Vector3)(distance * Direction);

                UnityEngine.Debug.DrawLine(colliderPosition + topLeftCast, colliderPosition + topRightCast, Color.yellow);
                UnityEngine.Debug.DrawLine(colliderPosition + topRightCast, colliderPosition + bottomRightCast, Color.yellow);
                UnityEngine.Debug.DrawLine(colliderPosition + bottomRightCast, colliderPosition + bottomLeftCast, Color.yellow);
                UnityEngine.Debug.DrawLine(colliderPosition + bottomLeftCast, colliderPosition + topLeftCast, Color.yellow);

                UnityEngine.Debug.DrawLine(colliderPosition, colliderPosition + (Vector3)(distance * Direction), Color.red);

            }

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
                    TriggeredHit = col.distance <= distance,
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
