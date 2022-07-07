using System.Collections.Generic;
using System.Linq;
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
            _collisionBlockers = new HashSet<Object>();
            GetFrameVariables();

            _collisionResults =
                new FrameVariableDefinition<CollisionResult[]>($"{gameObject.GetInstanceID()}_collisionDetector_triggers",
                    CheckCollision);
        }

        public void AttachPhysicsObject(LichtPhysicsObject obj)
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

        public bool IsBlocked => _collisionBlockers.Any();

        public CollisionResult[] Triggers => _frameVariables.Get(_collisionResults);

        public abstract CollisionResult[] CheckCollision();

        public abstract Vector2 Clamp();
    }
}
