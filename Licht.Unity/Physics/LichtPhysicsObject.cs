using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Licht.Impl.Memory;
using Licht.Unity.Extensions;
using Licht.Unity.Memory;
using UnityEngine;

namespace Licht.Unity.Physics
{
    public class LichtPhysicsObject : MonoBehaviour
    {
        public bool Static;
        public bool Ghost;
        public Vector2 VerticalColliderSize => VerticalCollider.bounds.extents;
        public Vector2 HorizontalColliderSize => HorizontalCollider.bounds.extents;

        public BoxCollider2D HorizontalCollider;
        public BoxCollider2D VerticalCollider;

        [Serializable]
        public struct CustomCollision
        {
            public LayerMask CollisionLayerMask;
            public string CollisionName;
            public BoxCollider2D ColliderToUse;
        }

        public CustomCollision[] CustomCollisionChecks;

        private LichtPhysics _physics;
        private string PhysicsFrameVar => $"LichtPhysicsObject_{gameObject.name}";
        private readonly RaycastHit2D[] _collisionResults = new RaycastHit2D[10];

        private Dictionary<Type, object> _customObjects;

        public void AddCustomObject<T>(T obj) where T:class
        {
            _customObjects[typeof(T)] = obj;
        }

        public bool TryGetCustomObject<T>(out T obj) where T:class
        {
            if (!_customObjects.ContainsKey(typeof(T)))
            {
                obj = default;
                return false;
            }

            obj = _customObjects[typeof(T)] as T;
            return true;
        }

        public Vector2 Speed
        {
            get
            {
                return _physics.GetFrameVariables()
                    .Get(new FrameVariableDefinition<Vector2>(PhysicsFrameVar, () => Vector2.zero));
            }
            set
            {
                _physics.GetFrameVariables().Set(PhysicsFrameVar, value);
                _latestSpeed.Current = value;
            }
        }

        public Vector2 CalculatedSpeed;

        private Caterpillar<Vector2> _latestSpeed;
        public Vector2 LatestSpeed => _latestSpeed.Current;

        private Caterpillar<Vector2> _latestDirection;
        public Vector2 LatestDirection => _latestDirection.Current;

        private void Awake()
        {
            _customObjects = new Dictionary<Type, object>();
            _physics = this.GetLichtPhysics();
            _latestSpeed = new Caterpillar<Vector2>
            {
                TailSize = 1
            };

            _latestDirection = new Caterpillar<Vector2>
            {
                TailSize = 1
            };
        }

        public void ImplyDirection(Vector2 direction)
        {
            _latestDirection.Current = new Vector2(Mathf.Abs(direction.x) > 0 ? direction.x : _latestDirection.Current.x,
                Mathf.Abs(direction.y) > 0 ? direction.y : _latestDirection.Current.y);
        }

        public void ApplySpeed(Vector2 speed)
        {
            Speed += speed;
        }

        private void OnEnable()
        {
            _physics.AddPhysicsObject(this);
        }

        private void OnDisable()
        {
            _physics.RemovePhysicsObject(this);
        }

        public CollisionResult CustomCast(CustomCollision customCollision, Vector2 direction)
        {
            for (var index = 0; index < _collisionResults.Length; index++)
            {
                _collisionResults[index] = default;
            }

            Physics2D.BoxCastNonAlloc((Vector2)transform.position + customCollision.ColliderToUse.offset, customCollision.ColliderToUse.size,
                0, direction, _collisionResults,
                Speed.magnitude, customCollision.CollisionLayerMask);

            var hits = _collisionResults.Where(c => c.collider != HorizontalCollider && c.collider != VerticalCollider
                    && c != default)
                .ToArray();

            return new CollisionResult
            {
                Orientation = CollisionOrientation.Undefined,
                Hits = hits,
                TriggeredHit = hits.Length > 0
            };
        }

        public CollisionResult Cast(LayerMask layerMask, Vector2 direction)
        {
            for (var index = 0; index < _collisionResults.Length; index++)
            {
                _collisionResults[index] = default;
            }

            var colliderToUse = Mathf.Abs(direction.y) > 0 ? VerticalCollider : HorizontalCollider;

            if (!colliderToUse.enabled)
            {
                return new CollisionResult
                {
                    Orientation = Mathf.Abs(direction.y) > 0
                        ? CollisionOrientation.Vertical
                        : CollisionOrientation.Horizontal,
                    Hits = Array.Empty<RaycastHit2D>(),
                    TriggeredHit = false
                };
            }
            
            Physics2D.BoxCastNonAlloc((Vector2)transform.position + colliderToUse.offset, colliderToUse.size, 0, direction, _collisionResults,
                Speed.magnitude, layerMask);

            var hits = _collisionResults.Where(c => c.collider != HorizontalCollider && c.collider != VerticalCollider
                    && c != default)
                .ToArray();

            return new CollisionResult
            {
                Orientation = Mathf.Abs(direction.y) > 0 ? CollisionOrientation.Vertical : CollisionOrientation.Horizontal,
                Hits = hits,
                TriggeredHit = hits.Length > 0
            };
        }
    }
}
