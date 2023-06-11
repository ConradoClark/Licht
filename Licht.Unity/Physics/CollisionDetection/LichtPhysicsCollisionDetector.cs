using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Unity.Extensions;
using Licht.Unity.Memory;
using Licht.Unity.Objects;
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

        public Collider2D Collider;
        public virtual CollisionDetectorType DetectorType { get; protected set; }
        public LichtPhysicsObject PhysicsObject { get; private set; }

        private HashSet<Object> _collisionBlockers;
        private FrameVariables _frameVariables;
        private FrameVariableDefinition<CollisionResult[]> _collisionResults;
        protected LichtPhysics Physics;

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

        public void AttachPhysicsObject(LichtPhysicsObject obj)
        {
            PhysicsObject = obj;

            if (Collider != null) Physics.RegisterCollider(Collider, obj);
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

        [field:SerializeField]
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
}
