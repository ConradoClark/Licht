using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Licht.Unity.Physics.CollisionDetection
{
    public class Basic2DUnityCollider : LichtPhysicsCollisionDetector
    {
        public bool ShouldClamp;
        private List<CollisionResult> _internalResults;

        protected override void OnAwake()
        {
            base.OnAwake();
            _internalResults = new List<CollisionResult>();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            var contact = collision.GetContact(0);
            _internalResults.Add(new CollisionResult
            {
                Collider = collision.otherCollider,
                Detected = true,
                TriggeredHit = true,
                Direction = (contact.point - (Vector2)PhysicsObject.transform.position).normalized,
                Hit = new RaycastHit2D
                {
                    point = contact.point,
                    distance = (contact.point - (Vector2)PhysicsObject.transform.position).magnitude,
                    normal = contact.normal,
                }
            });
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            var remove = _internalResults.FirstOrDefault(r => r.Collider == collision.otherCollider);
            if (remove.Collider != null) _internalResults.Remove(remove);
        }

        public override CollisionResult[] CheckCollision()
        {
            return _internalResults.ToArray();
        }

        public override Vector2 Clamp()
        {
            if (!ShouldClamp) return PhysicsObject.transform.position;
            var results = Triggers;
            if (results == null || results.Length == 0 ||
                (PhysicsObject.CalculatedSpeed.magnitude == 0 && results.All(r => !Collider.Distance(r.Collider).isOverlapped))) return PhysicsObject.transform.position;

            return (from result in results select Collider.Distance(result.Collider)
                into distance let distanceToCollide = distance.pointB - distance.pointA
                where distance.isOverlapped select distanceToCollide)
                .Aggregate(PhysicsObject.transform.position, (current, distanceToCollide) 
                    => current + (Vector3)distanceToCollide);
        }
    }
}
