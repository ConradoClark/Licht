using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Unity.Extensions;
using Licht.Unity.Memory;
using Licht.Unity.Objects;
using Licht.Unity.PropertyAttributes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Licht.Unity.Physics.CollisionDetection
{

    public abstract class LichtPhysicsCollisionDetector : BaseGameObject
    {
        public enum CollisionDetectorType
        {
            PreUpdate,
            PostUpdate
        }
        public virtual CollisionDetectorType DetectorType { get; protected set; }
        public LichtPhysicsObject PhysicsObject { get; private set; }

        private HashSet<Object> _collisionBlockers;
        private FrameVariables _frameVariables;
        private FrameVariableDefinition<CollisionResult[]> _collisionResults;
        protected LichtPhysics Physics;
        public virtual Collider2D AssociatedCollider => null;

        private FrameVariables GetFrameVariables()
        {
            if (_frameVariables != null) return _frameVariables;
            _frameVariables = Object.FindObjectOfType<FrameVariables>();

            if (_frameVariables != null) return _frameVariables;
            var obj = new GameObject("frameVars");
            return _frameVariables = obj.AddComponent<FrameVariables>();
        }

        protected override void OnAwake()
        {
            base.OnAwake();

            Physics = this.GetLichtPhysics();
            _collisionBlockers = new HashSet<Object>();
            GetFrameVariables();

            _collisionResults =
                new FrameVariableDefinition<CollisionResult[]>($"{gameObject.GetInstanceID()}_collisionDetector_triggers",
                    CheckCollisionForFrame);
        }

        public virtual void AttachPhysicsObject(LichtPhysicsObject obj)
        {
            PhysicsObject = obj;
        }

        public void BlockCollisionDetection(Object source)
        {
            if (_collisionBlockers.Contains(source)) return;
            _collisionBlockers.Add(source);
        }

        public void UnblockCollisionDetection(Object source)
        {
            if (!_collisionBlockers.Contains(source)) return;
            _collisionBlockers.Remove(source);
        }

        [field: CustomLabel("Select if detected collisions should be solid (restrict movement)")]
        [field: SerializeField]
        public bool ShouldClamp { get; private set; }

        public bool IsBlocked => _collisionBlockers.Any();

        public CollisionResult[] Triggers => _frameVariables.Get(_collisionResults);

        private CollisionResult[] CheckCollisionForFrame()
        {
            return CheckCollision();
        }

        public CollisionResult[] CheckCollision()
        {
            if (PhysicsObject == null || IsBlocked) return Array.Empty<CollisionResult>();
            return CalculateCollision();
        }

        public abstract CollisionResult[] CalculateCollision();

        public abstract Vector2 Clamp();

        public IEnumerable<T> FindTriggersAsObjects<T>(bool triggeredOnDetection = false) where T : class
        {
            foreach (var trigger in Triggers)
            {
                if (!trigger.TriggeredHit || (triggeredOnDetection && !trigger.Detected)) continue;
                if (!Physics.TryGetCustomObjectByCollider<T>(trigger.Collider, out var target)) continue;
                yield return target;
            }
        }
    }

    public abstract class LichtPhysicsCollisionDetector<T> : LichtPhysicsCollisionDetector where T : Collider2D
    {
        [field: SerializeField]
        public virtual T Collider { get; protected set; }

        public override void AttachPhysicsObject(LichtPhysicsObject obj)
        {
            base.AttachPhysicsObject(obj);

            if (Collider != null) Physics.RegisterCollider(Collider, obj);
        }

        public override Collider2D AssociatedCollider => Collider;
    }
}
