using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Licht.Unity.Objects;
using Licht.Unity.PropertyAttributes;
using UnityEngine;

namespace Licht.Unity.Physics.CollisionDetection
{
    [AddComponentMenu("L. Collision: Unity Collider")]
    public class Basic2DUnityCollider : LichtPhysicsCollisionDetector<Collider2D>
    {
        [field: CustomLabel("Select this if collision detection should activate a Trigger.")]
        [field: SerializeField]
        public bool SetTriggerOnCollision { get; set; }

        [field: ShowWhen(nameof(SetTriggerOnCollision))]
        public ScriptIdentifier Trigger;

        private List<CollisionResult> _internalResults;

        protected override void OnAwake()
        {
            base.OnAwake();
            _internalResults = new List<CollisionResult>();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (SetTriggerOnCollision) PhysicsObject.SetPhysicsTrigger(Trigger, true, this);

            var contact = collision.GetContact(0);
            _internalResults.Add(new CollisionResult
            {
                Collider = collision.otherCollider,
                Detected = true,
                TriggeredHit = true,
                Direction = (contact.point - PhysicsObject.GetCurrentPosition()).normalized,
                Hit = new RaycastHit2D
                {
                    point = contact.point,
                    distance = (contact.point - PhysicsObject.GetCurrentPosition()).magnitude,
                    normal = contact.normal,
                }
            });
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (SetTriggerOnCollision) PhysicsObject.SetPhysicsTrigger(Trigger, false, this);
            var remove = _internalResults.FirstOrDefault(r => r.Collider == collision.otherCollider);
            if (remove.Collider != null) _internalResults.Remove(remove);
        }

        public override CollisionResult[] CalculateCollision()
        {
            return _internalResults.ToArray();
        }

        public override Vector2 Clamp()
        {
            if (!ShouldClamp) return PhysicsObject.GetCurrentPosition();
            var results = Triggers;
            if (results == null || results.Length == 0 ||
                (PhysicsObject.CalculatedSpeed.magnitude == 0 && results.All(r => !Collider.Distance(r.Collider).isOverlapped))) return PhysicsObject.GetCurrentPosition();

            return (from result in results select Collider.Distance(result.Collider)
                into distance let distanceToCollide = distance.pointB - distance.pointA
                where distance.isOverlapped select distanceToCollide)
                .Aggregate(PhysicsObject.GetCurrentPosition(), (current, distanceToCollide) 
                    => current + distanceToCollide);
        }
    }
}
