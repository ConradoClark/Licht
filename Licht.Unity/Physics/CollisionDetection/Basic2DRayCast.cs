using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Licht.Unity.Objects;
using Licht.Unity.PropertyAttributes;
using UnityEngine;

namespace Licht.Unity.Physics.CollisionDetection
{
    [AddComponentMenu("L!> Collision: 2D Ray Cast")]
    public class Basic2DRayCast : LichtPhysicsCollisionDetector
    {
        [field: CustomLabel("Select this if collision detection should activate a Trigger.")]
        [field: SerializeField]
        public bool SetTriggerOnCollision { get; set; }

        [field: ShowWhen(nameof(SetTriggerOnCollision))]
        public ScriptIdentifier Trigger;

        [field: CustomHeader("Parameters")]
        [field: CustomLabel("The distance of the Ray Cast")]
        public float Distance;
        [field: ShowWhen(nameof(SetTriggerOnCollision))]
        public float DistanceToTrigger;

        [field: CustomLabel("Select this if the Ray Cast direction should be dictated by the target's speed")]
        public bool UseSpeedAsDirection;

        [field:ShowWhen(nameof(UseSpeedAsDirection), true)]
        public Vector2 Direction;

        [field: CustomLabel("Filter collision by Layer Mask")]
        public LayerMask LayerMask;
        private readonly RaycastHit2D[] _collisionResults = new RaycastHit2D[10];

        [field: CustomLabel("Select this to restrict collision detection by normals.")]
        [field: SerializeField]
        public bool CheckNormals { get; set; }
        [field: ShowWhen(nameof(CheckNormals))]
        [field: SerializeField]
        public Vector2 NormalTarget { get; set; }

        [field: CustomLabel("Select this to increase the Ray Cast size by the target's speed.")]
        public bool IncreaseBySpeed;

        public Vector2 RayCastOffset;

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

            var noHits = Physics2D.RaycastNonAlloc((Vector2)transform.position + RayCastOffset,
                UseSpeedAsDirection ? PhysicsObject.CalculatedSpeed : Direction, _collisionResults,
                distance, LayerMask);

            if (noHits == 0)
            {
                if (SetTriggerOnCollision) PhysicsObject.SetPhysicsTrigger(Trigger, false, this);
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

            if (SetTriggerOnCollision) PhysicsObject.SetPhysicsTrigger(Trigger, results.Any(t=>t.TriggeredHit), this);
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
