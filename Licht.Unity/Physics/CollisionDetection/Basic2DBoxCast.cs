using System;
using System.Linq;
using Licht.Impl.Debug;
using Licht.Unity.Debug;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Licht.Unity.PropertyAttributes;
using UnityEngine;

namespace Licht.Unity.Physics.CollisionDetection
{
    [AddComponentMenu("L. Collision: 2D Box Cast")]
    public class Basic2DBoxCast : LichtPhysicsCollisionDetector<BoxCollider2D>
    {
        [field: CustomLabel("Select this to see the Debug Gizmos.")]
        [field: SerializeField]
        public bool Debug { get; set; }

        [field: CustomLabel("Select this if collision detection should activate a Trigger.")]
        [field: SerializeField]
        public bool SetTriggerOnCollision { get; set; }

        [field: ShowWhen(nameof(SetTriggerOnCollision))]
        [field: SerializeField]
        public ScriptIdentifier Trigger { get; set; }

        [field: CustomHeader("Parameters")]
        [field: SerializeField]
        [field: CustomLabel("The distance of the BoxCast")]
        public float Distance { get; set; }

        [field: SerializeField]
        [field: CustomLabel("The direction of the BoxCast")]
        public Vector2 Direction { get; set; }

        [field: SerializeField]
        [field: CustomLabel("Filter collision by Layer Mask")]
        public LayerMask LayerMask { get; set; }

        private readonly RaycastHit2D[] _collisionResults = new RaycastHit2D[10];

        [field: CustomLabel("Select this to restrict collision detection by normals.")]
        [field: SerializeField]
        public bool CheckNormals { get; set; }

        [field: ShowWhen(nameof(CheckNormals))]
        [field: SerializeField]
        public Vector2 NormalTarget { get; set; }

        [field: CustomLabel("Select this to increase the BoxCast size by the target's speed.")]
        [field: SerializeField]
        public bool IncreaseBySpeed { get; set; }

        private const float _tolerance = 0.1f;

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

            var size = IncreaseBySpeed ? PhysicsObject.CalculatedSpeed * Direction : Vector2.zero;

            var colliderPosition = Collider.transform.position + (Vector3)Collider.offset;

            var noHits = Physics2D.BoxCastNonAlloc((Vector2)colliderPosition + Direction * (distance * 0.5f),
                Collider.size + size,
                0, Direction, _collisionResults, distance * 0.5f, LayerMask);

            if (Debug)
            {
                DebugPresets.DrawRectangle(colliderPosition, Collider.size, Color.green);
                DebugPresets.DrawRectangle(colliderPosition + (Vector3)(distance * Direction) * 0.5f,
                    Collider.size + size, Color.yellow);
                UnityEngine.Debug.DrawLine(colliderPosition, colliderPosition + (Vector3)(distance * Direction) * 0.5f,
                    Color.yellow);
            }

            if (noHits == 0)
            {
                if (SetTriggerOnCollision) PhysicsObject.SetPhysicsTrigger(Trigger, false, this);
                return Array.Empty<CollisionResult>();
            }

            var results = _collisionResults
                .Where(col =>
                    col.collider != null && !PhysicsObject.CollisionDetectors.Select(c => c.AssociatedCollider)
                                             .Contains(col.collider)
                                         && !PhysicsObject.AdditionalColliders.Contains(col.collider)
                                         && (!CheckNormals || col.normal == NormalTarget)
                )
                .Select(col => new CollisionResult
                {
                    Detected = true,
                    TriggeredHit = Vector2.Distance(col.point,col.centroid) <= _tolerance ||
                                   (col.point - (Vector2)colliderPosition)
                        .RoundToPrecision(4).magnitude <= (Collider.size * Direction).magnitude,
                    Direction = col.normal,
                    Collider = col.collider,
                    Hit = col,
                })
                .ToArray();

            if (SetTriggerOnCollision) PhysicsObject.SetPhysicsTrigger(Trigger, results.Any(t => t.TriggeredHit), this);
            return results;
        }

        public override Vector2 Clamp()
        {
            if (!ShouldClamp) return PhysicsObject.CalculatedSpeed;
            var results = Triggers;
            if (results == null || results.Length == 0 || PhysicsObject.CalculatedSpeed.magnitude == 0)
                return PhysicsObject.CalculatedSpeed;

            var targetSpeed = PhysicsObject.CalculatedSpeed;

            foreach (var result in results)
            {
                var colliderPosition = Collider.transform.position + (Vector3)Collider.offset;

                var distance =
                    (result.Hit.point - (Vector2)colliderPosition)
                    .RoundToPrecision(4);

                var hasXComponent = Mathf.Abs(Direction.x) > 0;
                var hasYComponent = Mathf.Abs(Direction.y) > 0;

                if (Debug)
                {
                    DebugPresets.DrawEllipse(result.Hit.point, Vector3.forward, Vector3.up, 0.1f, 0.1f, 8, Color.red);
                    DebugPresets.DrawEllipse(distance, Vector3.forward, Vector3.up, 0.05f, 0.05f, 8, Color.yellow);
                }

                targetSpeed = new Vector2(Mathf.Clamp(targetSpeed.x,
                        hasXComponent ? -Mathf.Abs(distance.x) : PhysicsObject.CalculatedSpeed.x,
                        hasXComponent ? Mathf.Abs(distance.x) : PhysicsObject.CalculatedSpeed.x),
                    Mathf.Clamp(targetSpeed.y, hasYComponent ? -Mathf.Abs(distance.y) : PhysicsObject.CalculatedSpeed.y,
                        hasYComponent && (PhysicsObject.Sticky ||
                                          Math.Sign(PhysicsObject.CalculatedSpeed.y) == Math.Sign(Direction.y))
                            ? Mathf.Abs(distance.y)
                            : PhysicsObject.CalculatedSpeed.y));
            }

            return targetSpeed;
        }
    }
}