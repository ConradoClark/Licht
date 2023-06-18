using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Unity.Memory;
using Licht.Unity.Objects;
using Licht.Unity.PropertyAttributes;
using UnityEngine;

namespace Licht.Unity.Physics.CollisionDetection
{
    public class Basic2DCollisionDetector : LichtPhysicsCollisionDetector<Collider2D>
    {
        [field: SerializeField]
        public ContactFilter2D ContactFilter;
        private readonly List<Collider2D> _collisionResults = new List<Collider2D>();

        [field: CustomHeader("Triggers")]
        [field: CustomLabel("Select this if collision detection should activate a Trigger.")]
        [field: SerializeField]
        public bool SetTriggerOnCollision { get; set; }

        [field: ShowWhen(nameof(SetTriggerOnCollision))]
        [field: SerializeField]
        public ScriptIdentifier Trigger;

        [field: CustomLabel("Select this if hitting a ceiling should activate a Trigger.")]
        [field: SerializeField]
        public bool SetTriggerOnCeiling { get; set; }

        [field: ShowWhen(nameof(SetTriggerOnCeiling))]
        [field: SerializeField]
        public ScriptIdentifier HitCeilingIdentifier;

        [CustomHeader("Special Conditions")]
        [BeginFoldout("Prevent Collisions by Physics Object's Direction")]
        [field: SerializeField]
        public bool PreventByGoingUp;
        [field: SerializeField]
        public bool PreventByGoingDown;
        [field: SerializeField]
        public bool PreventByGoingRight;
        [field: SerializeField]
        public bool PreventByGoingLeft;

        [EndFoldout]
        [BeginFoldout("Prevent Collisions by Direction")]
        [field: SerializeField]
        public bool PreventUpClamp;
        [field: SerializeField]
        public bool PreventDownClamp;
        [field: SerializeField]
        public bool PreventRightClamp;
        [field: SerializeField]
        public bool PreventLeftClamp;

        protected override void OnAwake()
        {
            base.OnAwake();
            DetectorType = CollisionDetectorType.PostUpdate;
        }

        public override CollisionResult[] CalculateCollision()
        {
            var noHits = Collider.OverlapCollider(ContactFilter, _collisionResults);
            if (noHits == 0)
            {
                if (SetTriggerOnCollision) PhysicsObject.SetPhysicsTrigger(Trigger, false, this);
                return Array.Empty<CollisionResult>();
            }

            var ignoredColliders = new HashSet<Collider2D>(PhysicsObject.CollisionDetectors.Select(c => c.AssociatedCollider));
            ignoredColliders.UnionWith(PhysicsObject.AdditionalColliders);

            var results = _collisionResults
                .Where(col => !ignoredColliders.Contains(col))
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

            if (SetTriggerOnCollision) PhysicsObject.SetPhysicsTrigger(Trigger, results.Any(r => r.TriggeredHit), this);
            return results;
        }

        public override Vector2 Clamp()
        {
            if (SetTriggerOnCeiling)
            {
                PhysicsObject.SetPhysicsTrigger(HitCeilingIdentifier, false, this);
            }

            if (!ShouldClamp) return PhysicsObject.GetCurrentPosition();

            var results = Triggers;
            if (results == null || results.Length == 0 ||
                (PhysicsObject.CalculatedSpeed.magnitude == 0 && results.All(r => !Collider.Distance(r.Collider).isOverlapped))) return PhysicsObject.GetCurrentPosition();
            var clampedPosition = PhysicsObject.GetCurrentPosition();

            foreach (var result in results)
            {
                var distance = Collider.Distance(result.Collider);
                if (!distance.isOverlapped) continue;

                UpdateHitCeilingTrigger(distance);

                var clamp = CalculateClampVector(distance, result.Collider);
                clampedPosition += clamp;
            }

            return clampedPosition;
        }

        private void UpdateHitCeilingTrigger(ColliderDistance2D distance)
        {
            if (!SetTriggerOnCeiling) return;

            var isHittingCeiling = distance.pointA.y > PhysicsObject.GetCurrentPosition().y
                                && Vector2.Angle(distance.normal, Vector2.up) < 90;

            PhysicsObject.SetPhysicsTrigger(HitCeilingIdentifier, isHittingCeiling, this);
        }

        private Vector2 CalculateClampVector(ColliderDistance2D distance, Collider2D target)
        {
            Physics.TryGetCustomObjectByCollider(target, out ColliderPushOutHint hint);

            var frameVariables = Physics.GetFrameVariables();

            var registry = new FrameVariableDefinition<CollisionPositionClampRegister>(
                $"clamp_registry_{Collider.GetInstanceID()}",
                () => null);

            var frameRegistry = frameVariables.Get(registry);

            if (frameRegistry != null)
            {
                hint.HorizontalHintDirection = Math.Sign(frameRegistry.Clamp.x) switch
                {
                    1 => ColliderPushOutHint.PushOutDirection.Negative,
                    -1 => ColliderPushOutHint.PushOutDirection.Positive,
                    _ => hint.HorizontalHintDirection
                };

                hint.VerticalHintDirection = Math.Sign(frameRegistry.Clamp.y) switch
                {
                    1 => ColliderPushOutHint.PushOutDirection.Negative,
                    -1 => ColliderPushOutHint.PushOutDirection.Positive,
                    _ => hint.VerticalHintDirection
                };
            };

            var distanceToCollide = distance.pointB - distance.pointA;

            var preventBySpeed = (PreventByGoingRight && PhysicsObject.CalculatedSpeed.x > 0) ||
                                 (PreventByGoingLeft && PhysicsObject.CalculatedSpeed.x < 0) ||
                                 (PreventByGoingUp && PhysicsObject.CalculatedSpeed.y > 0) ||
                                 (PreventByGoingDown && PhysicsObject.CalculatedSpeed.y < 0);

            var preventLeft = PreventLeftClamp || preventBySpeed;
            var preventRight = PreventRightClamp || preventBySpeed;

            var clampedX = preventLeft && distanceToCollide.x < 0 ? 0 :
                preventRight && distanceToCollide.x > 0 ? 0 :
                distanceToCollide.x;

            clampedX = hint?.HorizontalHintDirection switch
            {
                ColliderPushOutHint.PushOutDirection.Positive => Mathf.Abs(clampedX),
                ColliderPushOutHint.PushOutDirection.Negative => -Mathf.Abs(clampedX),
                _ => clampedX
            };

            var preventDown = PreventDownClamp || preventBySpeed;
            var preventUp = PreventUpClamp || preventBySpeed;

            var clampedY = preventDown && distanceToCollide.y < 0 ? 0 :
                preventUp && distanceToCollide.y > 0 ? 0 :
                distanceToCollide.y;

            clampedY = hint?.VerticalHintDirection switch
            {
                ColliderPushOutHint.PushOutDirection.Positive => Mathf.Abs(clampedY),
                ColliderPushOutHint.PushOutDirection.Negative => -Mathf.Abs(clampedY),
                _ => clampedY
            };

            frameVariables.Set($"clamp_registry_{target.GetInstanceID()}", new CollisionPositionClampRegister
            {
                Origin = Collider,
                Target = target,
                Clamp = new Vector2(clampedX, clampedY)
            });

            return new Vector2(clampedX, clampedY);
        }
    }
}
