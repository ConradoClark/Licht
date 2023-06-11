using System;
using System.Linq;
using Licht.Unity.Debug;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using UnityEngine;

namespace Licht.Unity.Physics.CollisionDetection
{
    public class Basic2DBoxCast : LichtPhysicsCollisionDetector
    {
        [field:SerializeField]
        public bool Debug { get; set; }
        [field: SerializeField]
        public ScriptIdentifier TriggerIdentifier { get; set; }
        [field: SerializeField]
        public float Distance { get; set; }
        [field: SerializeField]
        public Vector2 Direction { get; set; }
        [field: SerializeField]
        public LayerMask LayerMask { get; set; }
        private readonly RaycastHit2D[] _collisionResults = new RaycastHit2D[10];

        [field: SerializeField]
        public bool CheckNormals { get; set; }
        [field: SerializeField]
        public Vector2 NormalTarget { get; set; }

        [field: SerializeField]
        public bool IncreaseBySpeed { get; set; }

        [field: SerializeField]
        public BoxCollider2D BoxCollider { get; set; }

        protected override void OnAwake()
        {
            base.OnAwake();
            Collider = BoxCollider;
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
            
            var size = IncreaseBySpeed ? PhysicsObject.CalculatedSpeed * Direction : Vector2.zero;

            var colliderPosition = BoxCollider.transform.position + (Vector3) BoxCollider.offset;

            var noHits = Physics2D.BoxCastNonAlloc(colliderPosition, BoxCollider.size + size, 
                0, Direction, _collisionResults, distance*0.5f, LayerMask);

            if (Debug)
            {
                DebugPresets.DrawRectangle(colliderPosition, BoxCollider.size, Color.green);
                DebugPresets.DrawRectangle(colliderPosition + (Vector3)(distance * Direction), BoxCollider.size + size, Color.yellow);
                UnityEngine.Debug.DrawLine(colliderPosition, colliderPosition + (Vector3)(distance * Direction), Color.yellow);
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
            if (!ShouldClamp) return PhysicsObject.CalculatedSpeed;
            var results = Triggers;
            if (results == null || results.Length == 0 || PhysicsObject.CalculatedSpeed.magnitude == 0) return PhysicsObject.CalculatedSpeed;

            var targetSpeed = PhysicsObject.CalculatedSpeed;

            foreach (var result in results)
            {
                var colliderPosition = BoxCollider.transform.position + (Vector3)BoxCollider.offset;

                var distance =
                    (result.Hit.point - (Vector2)colliderPosition)
                    .RoundToPrecision(4);

                var hasXComponent = Mathf.Abs(Direction.x) > 0;
                var hasYComponent = Mathf.Abs(Direction.y) > 0;

                if (Debug)
                {
                    DebugPresets.DrawEllipse(result.Hit.point, Vector3.forward,Vector3.up,0.1f,0.1f,8, Color.red); DebugPresets.DrawEllipse(distance, Vector3.forward, Vector3.up, 0.05f, 0.05f, 8, Color.yellow);
                }

                targetSpeed = new Vector2(Mathf.Clamp(targetSpeed.x,
                        hasXComponent ? -Mathf.Abs(distance.x) : PhysicsObject.CalculatedSpeed.x,
                        hasXComponent ? Mathf.Abs(distance.x) : PhysicsObject.CalculatedSpeed.x),
                    Mathf.Clamp(targetSpeed.y, hasYComponent ? -Mathf.Abs(distance.y) : PhysicsObject.CalculatedSpeed.y,
                        hasYComponent && (PhysicsObject.Sticky || Math.Sign(PhysicsObject.CalculatedSpeed.y) == Math.Sign(Direction.y)) ? Mathf.Abs(distance.y) : PhysicsObject.CalculatedSpeed.y));    
            }

            return targetSpeed;
        }
    }
}
